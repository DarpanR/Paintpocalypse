using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public abstract class WeaponModule<Tdef> : IWeaponModule, IstatSetTarget
    where Tdef : WeaponData
{
    //Current Stats
    [SerializeField]
    int level = 1;

    readonly Tdef data;
    readonly Transform firePoint;
    readonly string targetTag;

    readonly IFireMode fireMode;
    readonly IFirePointStrategy firePoints;
    readonly IAttackBehavior attack;
    readonly IDamageBehavior damage;
    readonly StatBroker stats;

    public int Level { get; private set; }
    public StatSet CurrentStats => stats.CurrentStats;
    // explicit interface impl so consumer sees only the base type
    
    public WeaponModule(Tdef data, Transform firePoint, string targetTag,
        IAttackBehavior attack, 
        IDamageBehavior damage,
        IFireMode fireMode,
        IFirePointStrategy firePoints) {
        Level = level;
        this.data = data;
        this.firePoint = firePoint;
        this.targetTag = targetTag;
        this.attack = attack;
        this.fireMode = fireMode;
        this.firePoints = firePoints;

        stats = new StatBroker(data.baseStats);

        //fireTimer = new FireRateTimer(CurrentStats[StatType.FireRate].value);
        //fireTimer.ChangeProgress(Random.value);
        //fireTimer.Start();
        //fireTimer.OnTimerStop += Fire;
    }

    public void TryFire() {
        fireMode.Tick(Time.deltaTime);
        stats.Tick(Time.deltaTime);

        if (fireMode.ShouldFire()) {
            foreach (var (pos, rot) in firePoints.GetFirePoints(firePoint)) {
                attack.Fire(pos, rot, CurrentStats, targetTag, damage);
            }
        }
    }

    ///
    /// <summary>Spawn bullets with velocity and direction.</summary>
    /// 
    protected virtual void Fire() {
        attack.Fire(
            firePoint.position,
            firePoint.rotation,
            CurrentStats,
            targetTag,
            damage);
    }

    //protected void Fire(Vector3 spawnPoint, Quaternion rotation) {
    //    GameObject go = projManager.Request(Data.projectile, Data.poolSize);
    //    go.transform.SetPositionAndRotation(spawnPoint, rotation);
    //    Projectile proj = go.GetComponent<Projectile>();

    //    proj.Init(
    //        CurrentStats.Clone(),
    //        targetTag,
    //        OperationFactory.GetOperation(
    //            Data.operationType,
    //            Data.affectedType,
    //            -CurrentStats[StatType.Damage].value
    //            ),
    //        Penetration
    //    );
    //    proj.onDestroyed = () => {
    //        ProjectileManager.Instance.Return(go);
    //    };
    //}

    public virtual bool Upgrade() {
        if (Level >= data.maxLevel) return false;
        Level++;
        
        stats.UpdateBaseStat(StatType.Damage, GetComputedProperties(StatType.Damage));
        stats.UpdateBaseStat(StatType.FireRate, GetComputedProperties(StatType.FireRate));

        //fireTimer.Reset(CurrentStats[StatType.FireRate].value);
        // Recalculate with new base stats while preserving modifiers
        return true;
    }

    public bool AddStatModifier(StatModData data) {
        return stats.Add(data);
    }

    float GetComputedProperties (StatType type) {
        return CurrentStats[type].value + data.LevelStats.GetValueOrDefault(type, 0f);
    }

    // -- Computed Properties (base + level up stat) --
    //public int Penetration =>
    //    data.basePenetration + data.luPenetration * (Level - 1);
    public int ProjectileCount =>
        data.baseProjectileCount + data.luProjectileCount * (Level - 1);
}

public class ConcreteWeaponModule<T> : WeaponModule<T> where T : WeaponData {
    public ConcreteWeaponModule(
        T data,
        Transform firePoint,
        string targetTag,
        IAttackBehavior attack,
        IDamageBehavior damage,
        IFireMode fireMode,
        IFirePointStrategy firePoints
    ) : base(data, firePoint, targetTag, attack, damage, fireMode, firePoints) { }
}

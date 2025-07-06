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

    protected string targetTag;
    protected Transform firePoint;
    protected MonoBehaviour runner;
    protected FireRateTimer fireTimer;
    protected StatBroker statBroker;
    protected Queue<GameObject> pool;

    ProjectileManager projManager => ProjectileManager.Instance;

    public int Level { get; private set; }
    public Tdef Data { get; private set; }
    public StatSet CurrentStats => statBroker.CurrentStats;
    // explicit interface impl so consumer sees only the base type
    
    public WeaponModule(Tdef data, Transform firePoint, MonoBehaviour runner, string targetTag) {
        Level = level;
        Data = data;
        this.firePoint = firePoint;
        this.runner = runner;
        this.targetTag = targetTag;

        statBroker = new StatBroker(data.baseStats);

        //// **Per-weapon projectile pool**:
        //pool = new Queue<GameObject>();

        //for (int i = 0; i < data.poolSize; i++) {
        //    var go = GameObject.Instantiate(data.projectile);
        //    go.SetActive(false);
        //    pool.Enqueue(go);
        //}
        fireTimer = new FireRateTimer(CurrentStats[StatType.FireRate].value);
        fireTimer.ChangeProgress(Random.value);
        fireTimer.Start();
        fireTimer.OnTimerStop += Fire;
    }

    public void TryFire() {
        fireTimer.Tick(Time.deltaTime);
        statBroker.Tick(Time.deltaTime);
    }

    ///
    /// <summary>Spawn bullets with velocity and direction.</summary>
    /// 
    protected virtual void Fire() {
        Fire(firePoint.position, firePoint.rotation);
    }

    protected void Fire(Quaternion rotation) {
        Fire(firePoint.position, rotation);
    }
    protected void Fire(Vector3 spawnPoint, Quaternion rotation) {
        GameObject go = projManager.Request(Data.projectile, Data.poolSize);
        go.transform.SetPositionAndRotation(spawnPoint, rotation);
        Projectile proj = go.GetComponent<Projectile>();

        proj.Init(
            CurrentStats.Clone(),
            targetTag,
            OperationFactory.GetOperation(
                Data.operationType,
                Data.affectedType,
                -CurrentStats[StatType.Damage].value
                ),
            Penetration
        );
        proj.onDestroyed = () => {
            ProjectileManager.Instance.Return(go);
        };
    }

    public virtual bool Upgrade() {
        if (Level >= Data.maxLevel) return false;
        Level++;
        
        statBroker.UpdateBaseStat(StatType.Damage, GetComputedProperties(StatType.Damage));
        statBroker.UpdateBaseStat(StatType.FireRate, GetComputedProperties(StatType.FireRate));

        fireTimer.Reset(CurrentStats[StatType.FireRate].value);
        // Recalculate with new base stats while preserving modifiers
        return true;
    }

    public bool AddStatModifier(StatModData data) {
        return statBroker.Add(data);
    }

    float GetComputedProperties (StatType type) {
        return CurrentStats[type].value + Data.LevelStats.GetValueOrDefault(type, 0f);
    }

    // -- Computed Properties (base + level up stat) --
    public int Penetration =>
        Data.basePenetration + Data.luPenetration * (Level - 1);
    public int ProjectileCount =>
        Data.baseProjectileCount + Data.luProjectileCount * (Level - 1);
}

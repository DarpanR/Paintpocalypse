using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public abstract class WeaponModule<Tdef> : IWeaponModule, IstatSetTarget
    where Tdef : WeaponDefinition
{
    //Current Stats
    [SerializeField]
    int level = 1;

    protected string targetTag;
    protected Transform firePoint;
    protected FireRateTimer fireTimer;
    protected StatBroker statBroker;
    protected Queue<GameObject> pool;

    public int Level { get; private set; }
    public Tdef Definition { get; }
    public StatSet CurrentStats => statBroker.CurrentStats;
    // explicit interface impl so consumer sees only the base type
    WeaponDefinition IWeaponModule.Definition => Definition;
    
    public WeaponModule(Tdef definition, Transform firePoint, string targetTag) {
        Level = level;
        Definition = definition;
        this.firePoint = firePoint;
        this.targetTag = targetTag;

        statBroker = new StatBroker(Definition.baseStats);

        // **Per-weapon projectile pool**:
        pool = new Queue<GameObject>();

        for (int i = 0; i < Definition.poolSize; i++) {
            var go = GameObject.Instantiate(definition.projectile);
            go.SetActive(false);
            pool.Enqueue(go);
        }
        fireTimer = new FireRateTimer(CurrentStats[StatType.FireRate].value);
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
        GameObject go = pool.Count > 0 ?
            pool.Dequeue() :
            GameObject.Instantiate(Definition.projectile);
        go.transform.SetPositionAndRotation(spawnPoint, rotation);

        Projectile proj = go.GetComponent<Projectile>();

        proj.Init(
            CurrentStats.Clone(),
            targetTag,
            OperationFactory.GetOperation(
                Definition.operationType,
                Definition.affectedType,
                -CurrentStats[StatType.Damage].value
                ),
            Penetration
        );
        proj.onDestroyed = () => {
            ProjectileManager.Instance.Return(go);
        };
    }

    public virtual void Upgrade() {
        if (Level < Definition.maxLevel)
            Level++;
        statBroker.UpdateBaseStat(StatType.Damage, GetComputedProperties(StatType.Damage));
        statBroker.UpdateBaseStat(StatType.FireRate, GetComputedProperties(StatType.FireRate));

        fireTimer.Reset(CurrentStats[StatType.FireRate].value);
        // Recalculate with new base stats while preserving modifiers
    }

    public bool AddStatModifier(StatModifier modifier) {
        return statBroker.Add(modifier);
    }

    float GetComputedProperties (StatType type) {
        return CurrentStats[type].value + Definition.LevelStats.GetValueOrDefault(type, 0f);
    }

    // -- Computed Properties (base + level up stat) --
    public int Penetration =>
        Definition.basePenetration + Definition.luPenetration * (Level - 1);
    public int ProjectileCount =>
        Definition.baseProjectileCount + Definition.luProjectileCount * (Level - 1);
}

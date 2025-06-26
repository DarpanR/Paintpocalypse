using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public abstract class WeaponModule<Tdef> : IWeaponModule, IstatSetTarget
    where Tdef : WeaponDefinition
{
    public Tdef Definition { get; }
    // explicit interface impl so consumer sees only the base type
    WeaponDefinition IWeaponModule.Definition => Definition;

    protected StatBroker statBroker;
    public StatBroker StatBroker => statBroker;
    public StatSet CurrentStats => statBroker.CurrentStats;

    //Current Stats
    [HideInInspector]
    public int Level { get; private set; }

    protected StatBroker damageBroker;
    protected Transform firePoint;
    protected CountdownTimer fireTimer;
    protected Queue<GameObject> pool;
    // Tag reference for projectile targeting
    protected string target;

    public WeaponModule(Tdef def, Transform fp, string target) {
        Definition = def;
        this.target = target;
        firePoint = fp;
        Level = 1;

        StatSet baseStats = new StatSet();
        baseStats.AddStat(StatType.Damage, Definition.baseDamage);
        baseStats.AddStat(StatType.FireRate, Definition.baseFireRate);
        // Add other desired stats: Velocity, Penetration, etc. as needed
        statBroker = new StatBroker(baseStats);

        // **Per-weapon projectile pool**:
        pool = new Queue<GameObject>();

        for (int i = 0; i < Definition.poolSize; i++) {
            var go = GameObject.Instantiate(Definition.projectile);
            go.SetActive(false);
            pool.Enqueue(go);
        }
        fireTimer = new CountdownTimer(CurrentStats[StatType.FireRate].value);
        fireTimer.Start();
    }

    public void TryFire() {
        fireTimer.Tick(Time.deltaTime);

        if (fireTimer.IsFinished) { 
            fireTimer.Reset();
            Fire ();
        }
        statBroker.Tick(Time.deltaTime);
    }

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
        go.SetActive(true);

        var proj = go.GetComponent<Projectile>();

        proj.Init(
                rotation * Velocity,
                OperationFactory.GetOperation(
                    Definition.operationType,
                    Definition.affectedType,
                    -CurrentStats[StatType.Damage].value
                    ),
                LifeTime,
                Penetration,
                CurrentStats[StatType.FireRate].value,
                target
            );
        proj.onDestroyed = () => {
            go.SetActive(false);
            pool.Enqueue(go);
        };
    }

    public virtual void Upgrade() {
        if (Level < Definition.maxLevel)
            Level++;
        statBroker.UpdateBaseStat(StatType.Damage, Damage);
        statBroker.UpdateBaseStat(StatType.FireRate, FireRate);
        // Recalculate with new base stats while preserving modifiers
    }

    // -- Computed Properties (base + level up stat) --
    public float Damage =>
        statBroker.CurrentStats[StatType.Damage].value + Definition.luDamage * (Level - 1);
    public float FireRate =>
        statBroker.CurrentStats[StatType.FireRate].value + Definition.luFireRate * (Level - 1);
    public Vector2 Velocity =>
        Definition.baseVelocity + Definition.luVelocity * (Level - 1);
    public float LifeTime =>
        Definition.baseLifetime + Definition.luLifetime * (Level - 1);
    public int Penetration =>
        Definition.basePenetration + Definition.luPenetration * (Level - 1);
    public int ProjectileCount =>
        Definition.baseProjectileCount + Definition.luProjectileCount * (Level - 1);
}

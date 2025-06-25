using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponModule<Tdef> : IWeaponModule, IstatSetTarget
    where Tdef : WeaponDefinition
{
    public Tdef Definition { get; }
    // explicit interface impl so consumer sees only the base type
    WeaponDefinition IWeaponModule.Definition => Definition;

    protected StatBroker statBroker;
    public StatSet Stats => statBroker.CurrentStats;
    public StatBroker StatBroker => statBroker;
    

    //Current Stats
    [HideInInspector]
    public int Level { get; private set; }
    protected StatBroker damageBroker;
    protected Transform firePoint;
    protected StopwatchTimer fireTimer;
    protected Queue<GameObject> pool;
    // Tag reference for projectile targeting
    protected string Target;
    float interval;

    public WeaponModule(Tdef def, Transform fp, string t) {
        StatSet baseStats = new StatSet();
        baseStats.AddStat(StatType.Damage, Damage);
        baseStats.AddStat(StatType.FireRate, FireRate);
        // Add other desired stats: Velocity, Penetration, etc. as needed
        statBroker = new StatBroker(baseStats);

        Definition = def;
        Target = t;
        firePoint = fp;
        Level = 1;
        fireTimer = new StopwatchTimer();
        // reverse firerate
        interval = 1f / Stats[StatType.FireRate].value;
        fireTimer.Start();

        // **Per-weapon projectile pool**:
        pool = new Queue<GameObject>();

        for (int i = 0; i < Definition.poolSize; i++) {
            var go = GameObject.Instantiate(Definition.projectile);
            go.SetActive(false);
            pool.Enqueue(go);
        }
    }

    public void TryFire() {
        if (fireTimer.Progress > interval) {
            fireTimer.Reset();

            Fire ();
        }
    }

    /// <summary>Spawn bullets with velocity and direction.</summary>
    /// 
    protected virtual void Fire() {
        Fire(firePoint.position, firePoint.rotation);
    }
    protected void Fire(Quaternion rotation) {
        Fire(firePoint.position, rotation);
    }

    protected void Fire (Vector3 spawnPoint, Quaternion rotation) {
        GameObject go = pool.Count > 0 ?
            pool.Dequeue() :
            GameObject.Instantiate(Definition.projectile);
        go.transform.SetPositionAndRotation(spawnPoint, rotation);
        go.SetActive(true);

        var proj = go.GetComponent<Projectile>();

        proj.Init(
                rotation * Velocity,
                (int)Stats[StatType.Damage].value,
                LifeTime,
                Penetration,
                Stats[StatType.FireRate].value,
                Target
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
        Definition.baseDamage + Definition.luDamage * (Level - 1);
    public float FireRate =>
        Definition.baseFireRate + Definition.luFireRate * (Level - 1);
    public Vector2 Velocity =>
        Definition.baseVelocity + Definition.luVelocity * (Level - 1);
    public float LifeTime =>
        Definition.baseLifetime + Definition.luLifetime * (Level - 1);
    public int Penetration =>
        Definition.basePenetration + Definition.luPenetration * (Level - 1);
    public int ProjectileCount =>
        Definition.baseProjectileCount + Definition.luProjectileCount * (Level - 1);
}

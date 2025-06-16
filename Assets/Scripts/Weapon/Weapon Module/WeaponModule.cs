using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponModule<Tdef> : IWeaponModule
    where Tdef : WeaponDefinition
{
    public Tdef Definition { get; }
    // explicit interface impl so consumer sees only the base type
    WeaponDefinition IWeaponModule.Definition => Definition;

    //Current Stats
    [HideInInspector]
    public int Level { get; private set; }
    protected Transform firePoint;
    protected float fireTimer;
    protected Queue<GameObject> pool;
    // Tag reference for projectile targeting
    protected string Target;

    public WeaponModule(Tdef def, Transform fp, string t) {
        Definition = def;
        Target = t;
        firePoint = fp;
        Level = 1;
        fireTimer = 0f;

        // **Per-weapon pool**:
        pool = new Queue<GameObject>();

        for (int i = 0; i < Definition.poolSize; i++) {
            var go = GameObject.Instantiate(Definition.projectile);
            go.SetActive(false);
            pool.Enqueue(go);
        }
    }


    public void TryFire() {
        fireTimer += Time.deltaTime;
        float interval = 1f / FireRate;

        if (fireTimer > interval) {
            fireTimer -= interval;

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
                (int)Damage,
                LifeTime,
                Penetration,
                FireRate,
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

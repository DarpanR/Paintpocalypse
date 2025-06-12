using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Custom/Weapons/AOE Weapon")]
public class AOEDefinition : WeaponDefinition {
    [Header("Advanced Projectile Setting")]
    [Tooltip("Size of the effect")]
    public float aoeRadius = 2f;
    [Tooltip("Size of the location of the effects")]
    public float spawnRadius = 1.5f;

    [Header("Advanced Level Stats")]
    public int luAoeRadius = 0;
    public int luSpawnRadius = 0;

    public override IWeaponModule CreateModule(Transform firePoint, string target) {
        return new AreaOfEffect(this, firePoint, target);
    }
}
public class AreaOfEffect : WeaponModule<AOEDefinition> {
    public AreaOfEffect(AOEDefinition def, Transform fp, string target):base(def, fp, target) { }

    protected override void Fire() {
        for (int i = 0; i < ProjectileCount;i++) {
            Vector2 offset = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = firePoint.position + (Vector3)offset;

            Vector2 direction = (Vector2)offset.normalized;
            Quaternion rot = Quaternion.LookRotation(Vector3.forward, direction);

            Fire(spawnPos, rot);
        }
    }

    public float AoeRadius =>
        Definition.aoeRadius + Definition.luAoeRadius * (Level - 1);
    public float spawnRadius =>
        Definition.spawnRadius + Definition.luSpawnRadius * (Level - 1);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

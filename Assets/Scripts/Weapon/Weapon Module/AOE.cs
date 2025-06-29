using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaOfEffect : WeaponModule<AOEData> {
    public AreaOfEffect(AOEData def, Transform fp, MonoBehaviour r, string targetTag):base(def, fp, r, targetTag) { }

    protected override void Fire() {
        for (int i = 0; i < ProjectileCount;i++) {
            Vector2 offset = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = firePoint.position + (Vector3)offset;

            Vector2 direction = offset.normalized;
            Quaternion rot = Quaternion.LookRotation(Vector3.forward, direction);
            Fire(spawnPos, rot);
        }
    }

    public float spawnRadius =>
        data.spawnRadius + data.luSpawnRadius * (Level - 1);
}

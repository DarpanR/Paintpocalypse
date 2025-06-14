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

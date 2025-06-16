using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Weapons/Spread Shot")]
public class SpreadDefinition : WeaponDefinition {
    [Header("Advanced Projectile Setting")]
    public float baseSpreadAngle = 30f;

    [Header("Advanced Level Stats")]
    public float luSpreadAngle = 0f;

    public override IWeaponModule CreateModule(Transform firePoint, string target) {
        return new Spread(this, firePoint, target);
    }
}

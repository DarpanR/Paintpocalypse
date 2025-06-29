using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Weapons/Shotgun")]
public class ShotgunData : WeaponData {
    [Header("Advanced Projectile Setting")]
    public float baseSpreadAngle = 30f;

    [Header("Advanced Level Stats")]
    public float luSpreadAngle = 0f;

    public override IWeaponModule CreateModule(Transform firePoint, MonoBehaviour runner, string targetTag) {
        return new Shotgun(this, firePoint, runner, targetTag);
    }
}

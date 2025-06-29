using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Weapons/Automatic")]
public class AutomaticData : WeaponData {
    [Header("Advanced Projectile Setting")]
    public float baseSpreadAngle = 30f;
    public bool semi = false;
    public float offset = 5f;

    [Header("Advanced Level Stats")]
    public float luSpreadAngle = 0f;
    public float luOffset = 5f;

    public override IWeaponModule CreateModule(Transform firePoint, MonoBehaviour runner, string targetTag) {
        return new Automatic(this, firePoint, runner, targetTag);
    }
}

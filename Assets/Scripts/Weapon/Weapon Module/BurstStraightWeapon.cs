using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBurstWeapon", menuName ="Custom/BurstWeapon")]
public class BurstStraightWeapon : WeaponData
{
    public override IWeaponModule CreateModule(Transform firePoint, string targetTag) {
        float fireRate = baseStats.GetValueOrDefault(StatType.FireRate, 1f);

        return new ConcreteWeaponModule<BurstStraightWeapon>
    }
}

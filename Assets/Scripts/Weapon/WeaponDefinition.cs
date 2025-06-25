using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public abstract class WeaponDefinition : ScriptableObject, IPickupDefinition {
    [Header("Pickup Setting")]
    public Sprite pickupIcon;
    [Min(1)]
    public int amount = 1;
    public PickupType pickupType = PickupType.Weapon;

    [Header("Projectile Settings")]
    public GameObject projectile;
    public int poolSize = 20;

    [Header("Base Stats")]
    public float baseDamage = 10f;
    public float baseFireRate = 1f;
    public Vector2 baseVelocity = new Vector2();
    public float baseLifetime = 2f;
    public int basePenetration = 1;
    public int baseProjectileCount = 1;
    public int maxLevel = 5;

    [Header("Levelling Stats")]
    public float luDamage = 10f;
    public float luFireRate = 1f;
    public Vector2 luVelocity = new Vector2();
    public float luLifetime = 2f;
    public int luPenetration = 1;
    public int luProjectileCount = 1;

    public PickupType PickupType => pickupType;
    public Sprite PickupIcon => pickupIcon;
    public int Amount => amount;

    public abstract IWeaponModule CreateModule(Transform firePoint, string target);

}
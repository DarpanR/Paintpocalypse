using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public abstract class WeaponData : ScriptableObject, IPickupData {
    [Header("Base Stats")]
    public string weaponName;
    public int maxLevel = 5;
    public StatType affectedType = StatType.CurrentHealth;
    public StatSet baseStats = new StatSet(
        new Stat(StatType.Damage, 10f),
        new Stat(StatType.FireRate, 1f),
        new Stat(StatType.Speed, 5f),
        new Stat(StatType.Lifetime, 2f),
        new Stat(StatType.Penetration, 1f)
    );
    public int baseProjectileCount = 1;

    [Header("Levelling Stats")]
    public StatSet LevelStats = new StatSet(
        new Stat (StatType.Damage, 10)
    );
    public int luPenetration = 1;
    public int luProjectileCount = 1;

    [Header("Projectile Settings")]
    public GameObject projectile;
    public int poolSize = 20;

    [Header("Pickup Setting")]
    public Sprite pickupIcon;
    [TagMaskField]
    public string pickupTag;
    [Min(-1)]
    public int totalUsage = 1;
    [Min(-1)]
    public float lifetime = 5f;
    public TargetingType dropOperationType = TargetingType.OverMouse;
    public float dropRadius = 3f;

    public abstract IWeaponModule CreateModule(Transform firePoint, string targetTag);

    public String PickupName => weaponName;
    public Sprite PickupIcon => pickupIcon;
    public Sprite DropIcon => pickupIcon;
    public string PickupTag => pickupTag;
    public PickupType PickupType => PickupType.Weapon;
    public float LifeTime => lifetime;
    public TargetingType TargetingType => dropOperationType;
    public int TotalUsage => totalUsage;
    public float TargetRadius => dropRadius;
}
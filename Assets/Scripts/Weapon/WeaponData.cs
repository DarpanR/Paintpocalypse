using System;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName ="Weapon Data", menuName ="Custom/Weapon Data")]
public class WeaponData : ScriptableObject, IPickupData {
    [Header("Weapon Settings")]
    public string weaponName;
    public Sprite weaponIcon;

    [Header("Weapon Logic")]
    public FireModeType fireMode;
    public AttackBehaviorType attackBehavior;
    public DamageBehaviorType damageBehavior;
    public FirePointBehaviorType fireBehavior;
    public FiringPatternType firingPattern;
    public List<FirePoint> firePoints = new List<FirePoint>() {
        new FirePoint{
            position=Vector3.zero,
            angle=0 
        }
    };

    [Header("Base Stats")]
    public int maxLevel = 5;
    public StatType affectedType = StatType.CurrentHealth;
    public StatSet baseStats = new StatSet(
        new Stat(StatType.Damage, 10f),
        new Stat(StatType.FireRate, 1f),
        new Stat(StatType.Speed, 5f),
        new Stat(StatType.Lifetime, 2f),
        new Stat(StatType.Penetration, 1f),
        new Stat(StatType.ProjectileCount, 1f),
        new Stat(StatType.Cooldown, 1f),
        new Stat(StatType.FirePointAngle, 30f)
    );

    [Header("Levelling Stats")]
    public StatSet LevelStats = new StatSet(
        new Stat (StatType.Damage, 10)
    );

    [Header("Projectile Settings")]
    public GameObject projectile;
    public int poolSize = 20;

    [Header("Pickup Setting")]
    public PickupData pickupData = new PickupData {
        totalUsage = 1,
        lifeTime = 4f,
        dropOperationType = TargetingType.OverMouse,
        dropRadius = 3f
    };

    public String PickupName => weaponName;
    public Sprite PickupIcon => pickupData.pickupIcon;
    public Sprite DropIcon => pickupData.pickupIcon;
    public string PickupTag => pickupData.pickupTag;
    public PickupType PickupType => PickupType.Weapon;
    public float LifeTime => pickupData.lifeTime;
    public TargetingType TargetingType => pickupData.dropOperationType;
    public int TotalUsage => pickupData.totalUsage;
    public float TargetRadius => pickupData.dropRadius;
}
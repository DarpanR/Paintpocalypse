using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public abstract class WeaponData : ScriptableObject, IPickupData {
    [Header("Pickup Setting")]
    public Sprite pickupIcon;
    [TagMaskField]
    public string pickupTag;
    [Min(1)]
    public int pickupCount = 1;
    [Min(-1)]
    public int totalUsage = 1;
    [Min(-1)]
    public float lifetime = 5f;
    public DropType dropOperationType = DropType.OverMouse;

    public float dropRadius = 3f;
    public float dropForce = 5f;

    [Header("Magnifying Glass Setting")]
    public float sizeMultiplier = 2f;
    public float damageMultiplier = 2f;
    public float speedMultplier = 2f;

    [Header("Projectile Settings")]
    public GameObject projectile;
    public int poolSize = 20;

    [Header("Base Stats")]
    public string weaponName;
    public int maxLevel = 5;
    public OperationType operationType = OperationType.Addition;
    public StatType affectedType = StatType.CurrentHealth;
    public StatSet baseStats = new StatSet(
        new Stat(StatType.Damage, 10f),
        new Stat(StatType.FireRate, 1f),
        new Stat(StatType.Speed, 5f),
        new Stat(StatType.Lifetime, 2f)
    );
    public int basePenetration = 1;
    public int baseProjectileCount = 1;

    [Header("Levelling Stats")]
    public StatSet LevelStats = new StatSet(
        new Stat (StatType.Damage, 10)
    );
    public int luPenetration = 1;
    public int luProjectileCount = 1;
    
    [SerializeField, HideInInspector]
    string guid = Guid.NewGuid().ToString();

    public string GUID {
        get {
            if (string.IsNullOrEmpty(guid))
                guid = Guid.NewGuid().ToString();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
            return guid;
        }
    }

    public abstract IWeaponModule CreateModule(Transform firePoint, MonoBehaviour runner, string targetTag);

    public String DisplayName => weaponName;
    public Sprite PickupIcon => pickupIcon;
    public Sprite DropIcon => pickupIcon;
    public string PickupTag => pickupTag;
    public PickupType PickupType => PickupType.Weapon;
    public float LifeTime => lifetime;
    public int RemainingUsage => totalUsage;
    public DropType DropType => dropOperationType;
    public float PickupCount => pickupCount;
    public int TotalUsage => TotalUsage;
    public float DropRadius => dropRadius;
    public float DropForce => dropForce;
}
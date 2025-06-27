using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class WeaponDefinition : ScriptableObject, IPickupDefinition {
    [Header("Pickup Setting")]
    public Sprite pickupIcon;
    [Min(1)]
    public int pickupCount = 1;
    [TagMaskField]
    public string pickupTag;
    public DropType dropType = DropType.Counter;
    [Min(1)]
    public int dropCount = 1;

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
        new Stat(StatType.Velocity, 5f)
    );
    public float baseLifetime = 2f;
    public int basePenetration = 1;
    public int baseProjectileCount = 1;

    [Header("Levelling Stats")]
    public StatSet LevelStats = new StatSet(
        new Stat (StatType.Damage, 10)
    );
    public float luLifetime = 2f;
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

    public String DisplayName => weaponName;
    public Sprite PickupIcon => pickupIcon;
    public Sprite DropIcon => pickupIcon;
    public string PickupTag => pickupTag;
    public PickupType PickupType => PickupType.Weapon;
    public DropType DropType => dropType;
    public int DropCount => dropCount;
    public float PickupCount => pickupCount;
    public abstract IWeaponModule CreateModule(Transform firePoint, string target);

}
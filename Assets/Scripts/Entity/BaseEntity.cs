using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class BaseEntity : MonoBehaviour, IstatSetTarget, IWeaponManagerTarget {
    public EntityData entityData;
    public SpriteRenderer rend;

    protected EntityData eData;

    [SerializeField]
    StatFlasher flasher;

    protected StatBroker statBroker;
    WeaponManager weaponManager;

    protected bool isInvincible = false;

    public event Func<StatModData, bool> OnAddStatModifier;

    public string TargetTag => eData.targetTag;
    public string GUID { get; private set; }
    public StatSet CurrentStats => statBroker.CurrentStats;
    public WeaponManager WeaponManager => weaponManager;

    protected virtual void Awake() {
        flasher = flasher != null ? flasher : GetComponent<StatFlasher>();

        if (flasher == null)
            throw new NullReferenceException("flasher missing homeslice");
    }

    protected virtual void Start() {
        if (entityData != null) Init(entityData, entityData.GUID);
        flasher.Init(eData.VisualStatusEffects);
        statBroker.UpdateStats += OnStatUpdated;
        isInvincible = false;
    }

    protected virtual void LateUpdate() {
        weaponManager.Update();
        statBroker.Tick(Time.deltaTime);
    }

    public virtual void Init(EntityData entityData, string guid) {
        eData = entityData;
        GUID = guid;

        //EntityManager.Instance.RegisterEntity(guid, eData);

        statBroker = new StatBroker(InitializeStat());
        weaponManager = new WeaponManager(transform, eData.loadOutWeapons, eData.targetTag, this);
    }

    StatSet InitializeStat() {
        StatSet sSet = eData.baseStats.Clone();

        // Ensure MaxHealth exists before accessing its value
        float maxHealth = sSet.GetValueOrAdd(StatType.MaxHealth, 100f);

        // Then initialize CurrentHealth if missing
        sSet.GetValueOrAdd(StatType.CurrentHealth, maxHealth);
        // Set LocalScale
        sSet.GetValueOrAdd(StatType.LocalScale, 1f);
        sSet.GetValueOrAdd(StatType.Speed, 2f);

        return sSet;
    }

    public virtual void TakeDamage(IoperationStrategy operation) {
        if (isInvincible) return;
        float duration = CurrentStats.GetValueOrDefault(StatType.InvincibilityDuration, 0f);

        if (duration > 0) {
            isInvincible = true;
            flasher?.Trigger(StatEffectType.Damage, duration, () => isInvincible = false);
        } else {
            flasher?.Trigger(StatEffectType.Damage, 0.2f);
        }
        statBroker.UpdateBaseStat(operation);
    }

    protected virtual void OnStatUpdated() {
        if (CurrentStats[StatType.CurrentHealth].value <= 0) Die();
    }

    public bool AddStatModifier(StatModData def) {
        bool weaponCapable = (def.Capabilities & ModifierCapabilities.Weapon) != 0;

        if (weaponCapable) {
            foreach (var weapon in weaponManager.Weapons)
                if (weapon is IstatSetTarget statTarget)
                    statTarget.AddStatModifier(def);
        }
        OnAddStatModifier?.Invoke(def);
        return statBroker.Add(def);
    }

    protected virtual void Die() {
        flasher.OnDestroy();
        //Debug.Log(gameObject.name + " Died");
        //TODO: GAME OVER SCREEN TRIGGER

        Destroy(gameObject);
    }
}

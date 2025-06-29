using System;
using UnityEngine;

public abstract class BaseEntity : MonoBehaviour, IstatSetTarget, IWeaponManagerTarget {
    public EntityData eData;
    [SerializeField]
    StatFlasher flasher;

    StatBroker statBroker;
    WeaponManager weaponManager;

    bool isInvincible = false;

    public event Action OnTakeDamage;
    public event Func<StatModData, bool> OnAddStatModifier;
    public event Action<BaseEntity> OnDie;

    public StatSet CurrentStats => statBroker.CurrentStats;
    public WeaponManager WeaponManager => weaponManager;

    protected virtual void Awake() {
        flasher = flasher ?? GetComponent<StatFlasher>();

        if (flasher == null)
            throw new NullReferenceException("flasher missing homeslice");
        statBroker = new StatBroker(InitializeStat());
        weaponManager = new WeaponManager(transform, eData.loadOutWeapons, eData.targetTag, this);
    }

    protected virtual void Start() {
        flasher.Init(eData.VisualStatusEffects);
        OnDie += _ => flasher.OnDestroy();
        statBroker.UpdateStats += OnStatUpdated;
        isInvincible = false;
    }

    protected virtual void LateUpdate() {
        weaponManager.Update();
        statBroker.Tick(Time.deltaTime);
    }

    void OnDestroy() {
        //if (statBroker != null)
        //    statBroker.UpdateStats -= OnStatUpdated;
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
        if (isInvincible)
            return;
        float duration = CurrentStats.GetValueOrDefault(StatType.InvincibilityDuration, 0f);

        if (duration > 0) {
            isInvincible = true;
            flasher?.Trigger(StatEffectType.Damage, duration, () => isInvincible = false);
        } else {
            flasher?.Trigger(StatEffectType.Damage, 0.1f);
        }
        statBroker.UpdateBaseStat(operation);
        OnTakeDamage?.Invoke();
    }

    protected virtual void OnStatUpdated() {
        var newValue = CurrentStats[StatType.LocalScale].value;
        transform.localScale = new Vector3(newValue, newValue);

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
        OnDie?.Invoke(this);
        //Debug.Log(gameObject.name + " Died");
        //TODO: GAME OVER SCREEN TRIGGER

        Destroy(gameObject);
    }
}

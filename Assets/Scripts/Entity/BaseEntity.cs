using System;
using UnityEngine;

public abstract class BaseEntity : MonoBehaviour, IVisitor, IstatSetTarget, IWeaponManagerTarget {
    public EntityData eData;
    [SerializeField]
    StatFlasher flasher;

    StatBroker statBroker;
    WeaponManager weaponManager;

    bool isInvincible = false;

    public event Action OnTakeDamage;
    //public event Action<int> OnUpgrade;
    public event Action<BaseEntity> OnDie;

    public StatSet CurrentStats => statBroker.CurrentStats;
    public WeaponManager WeaponManager => weaponManager;

    protected virtual void Awake() {
        flasher = flasher ?? GetComponent<StatFlasher>();

        if (flasher == null)
            throw new NullReferenceException("flasher missing homeslice");

        statBroker = new StatBroker(InitializeStat());

        weaponManager = new WeaponManager(transform, eData.loadOutWeapons, eData.targetTag);
    }

    protected virtual void Start() {
        flasher.Init(eData.VisualStatusEffects);
        statBroker.UpdateStats += OnStatUpdated;
        isInvincible = false;
    }

    protected virtual void LateUpdate() {
        statBroker.Tick(Time.deltaTime);
        weaponManager.Update();
    }

    void OnDestroy() {
        if (statBroker != null)
            statBroker.UpdateStats -= OnStatUpdated;
    }

    StatSet InitializeStat() {
        StatSet sSet = eData.baseStats.Clone();

        // Ensure MaxHealth exists before accessing its value
        float maxHealth = sSet.GetValueOrAdd(StatType.MaxHealth, 100f);

        // Then initialize CurrentHealth if missing
        if (!sSet.HasStat(StatType.CurrentHealth)) {
            sSet.AddStat(StatType.CurrentHealth, maxHealth);
        }
        // Set LocalScale
        sSet.GetValueOrAdd(StatType.LocalScale, 1f);
        sSet.GetValueOrAdd(StatType.Velocity, 2f);

        return sSet;
    }

    public virtual void TakeDamage(IoperationStrategy operation) {
        if (isInvincible)
            return;
        float duration = CurrentStats.GetValueOrDefault(StatType.InvincibilityDuration, 0.1f);

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

    protected virtual void Die() {
        Debug.Log(gameObject.name + " Died");
        //TODO: GAME OVER SCREEN TRIGGER

        OnDie?.Invoke(this);
        Destroy(gameObject);
    }

    public bool AddStatModifier(StatModifier modifier) {
        if (modifier is IWeaponModifier weapMod) {
            weapMod.Activate(WeaponManager);
        }
        return statBroker.Add(modifier);
    }

    public void Visit(IVisitable visitable) => visitable.Accept(this);
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(StatusFlasher))]
public abstract class BaseEntity : MonoBehaviour, IVisitor, IstatSetTarget, IWeaponManagerTarget {
    public SpriteRenderer rend;
    [SerializeField]
    StatusFlasher flasher;

    StatBroker statBroker;
    WeaponManager weaponManager;

    bool isInvincible = false;

    public event Action OnTakeDamage;
    //public event Action<int> OnUpgrade;
    public event Action<BaseEntity> OnDie;

    public StatSet CurrentStats => statBroker.CurrentStats;
    public WeaponManager WeaponManager => weaponManager;

    protected virtual void Awake() {
        statBroker = new StatBroker(InitializeStat());
        weaponManager = new WeaponManager(transform, allWeapons, targetTag);
        flasher = GetComponent<StatusFlasher>();
    }

    protected virtual void Start() {
        statBroker.UpdateStats += OnStatUpdated;

        if (rend == null) rend = GetComponent<SpriteRenderer>();
        flasher.rend = flasher.rend != null ? flasher.rend : rend;
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
        StatSet sSet = entityData.stats.Clone();

        // Ensure MaxHealth exists before accessing its value
        float maxHealth = sSet.GetValueOrAdd(StatType.MaxHealth, 100f);

        // Then initialize CurrentHealth if missing
        if (!sSet.HasStat(StatType.CurrentHealth)) {
            sSet.AddStat(StatType.CurrentHealth, maxHealth);
        }
        // Set LocalScale
        sSet.GetValueOrAdd(StatType.LocalScale, 1f);
        sSet.GetValueOrAdd(StatType.MoveSpeed, 2f);

        return sSet;
    }

    public virtual void TakeDamage(IoperationStrategy operation) {
        if (isInvincible) return;
        isInvincible = true;
        float duration = CurrentStats.GetValueOrDefault(StatType.InvincibitilityDuration, 0.1f);
        //Debug.Log(operation.Value);
        flasher?.Trigger(StatusEffectType.Damage, duration, () => isInvincible = false);
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

        OnDie.Invoke(this);
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

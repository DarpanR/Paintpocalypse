using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StatusFlasher))]
public abstract class BaseEntity : MonoBehaviour, IVisitor, IstatSetTarget, IWeaponManagerTarget {
    public string entityName;
    [SerializeField, TagMaskField]
    string target = "Untagged";
    // Only use in Serialize. Never in run time;
    [SerializeField]
    StatSet stats;
    [SerializeField]
    List<WeaponDefinition> weapons;


    [Header("Visual Settings")]
    public SpriteRenderer rend;
    public float statFlashSpeed = 0.1f;
    public Color dmgColor = Color.red;

    public event Action OnTakeDamage;
    //public event Action<int> OnUpgrade;
    public event Action<BaseEntity> OnDie;

    StatBroker statBroker;
    WeaponManager weaponManager;
    StatusFlasher flasher;

    bool isInvincible = false;

    public StatBroker StatBroker => statBroker;
    public StatSet CurrentStats => statBroker.CurrentStats;
    public WeaponManager WeaponManager => weaponManager;

    protected virtual void Awake() {
        InitializeStat();

        statBroker = new StatBroker(stats);
        weaponManager = new WeaponManager(transform, target, weapons);
        flasher = GetComponent<StatusFlasher>();
    }

    void InitializeStat() {
        // Ensure MaxHealth exists before accessing its value
        float maxHealth = stats.GetValueOrAdd(StatType.MaxHealth, 100f);

        // Then initialize CurrentHealth if missing
        if (!stats.HasStat(StatType.CurrentHealth)) {
            stats.AddStat(StatType.CurrentHealth, maxHealth);
        }
        // Set LocalScale
        stats.GetValueOrAdd(StatType.LocalScale, 1f);
        stats.GetValueOrAdd(StatType.MoveSpeed, 2f);
    }

    protected virtual void Start() {
        statBroker.UpdateStats += OnStatUpdated;

        if (rend == null) rend = GetComponent<SpriteRenderer>();
        flasher.rend = rend;
    }

    protected virtual void LateUpdate() {
        statBroker.Tick(Time.deltaTime);
        weaponManager.Update();
    }

    public virtual void TakeDamage(IoperationStrategy operation) {
        if (isInvincible) return;
        isInvincible = true;
        float duration = stats.GetValueOrDefault(StatType.InvincibitilityDuration, 0.1f);

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

    void OnDestroy() {
        if (statBroker != null) 
            statBroker.UpdateStats -= OnStatUpdated;
    }

    public bool AddStatModifier(StatModifier modifier) => statBroker.Add(modifier);

    public void Visit(IVisitable visitable) => visitable.Accept(this);
}

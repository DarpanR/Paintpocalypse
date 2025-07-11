using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;



#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class BaseEntity : MonoBehaviour, IWeaponManagerTarget {
    public EntityData entityData;
    public SpriteRenderer rend;

    protected EntityData data;

    [SerializeField]
    StatFlasher flasher;

    protected StatBroker<EntityStatType> statBroker;
    WeaponManager weaponManager;

    protected bool isInvincible = false;

    public event Action<StatModData> OnAddStatModifier;

    public string TargetTag => data.targetTag;
    public string GUID { get; private set; }
    public StatSet<EntityStatType> CurrentStats => statBroker.CurrentStats;
    public WeaponManager WeaponManager => weaponManager;

    protected virtual void Awake() {
        flasher = flasher != null ? flasher : GetComponent<StatFlasher>();

        if (flasher == null)
            throw new NullReferenceException("flasher missing homeslice");
    }

    protected virtual void Start() {
        if (entityData != null) Init(entityData, entityData.GUID);
        flasher.Init(data.VisualStatusEffects);
        statBroker.UpdateStats += OnStatUpdated;
        isInvincible = false;
    }

    protected virtual void LateUpdate() {
        weaponManager.Update();
        statBroker.Tick(Time.deltaTime);
    }

    public virtual void Init(EntityData entityData, string guid) {
        data = entityData;
        GUID = guid;

        //EntityManager.Instance.RegisterEntity(guid, data);

        statBroker = new StatBroker<EntityStatType>(InitializeStat());
        weaponManager = new WeaponManager(transform, data.loadOutWeapons, data.targetTag);

        OnAddStatModifier += weaponManager.AddStatModifier;
    }

    StatSet<EntityStatType> InitializeStat() {
        StatSet<EntityStatType> sSet = data.baseStats.Clone();

        // Ensure MaxHealth exists before accessing its value
        float maxHealth = sSet.GetValueOrAdd(EntityStatType.MaxHealth, 100f);

        // Then initialize CurrentHealth if missing
        sSet.GetValueOrAdd(EntityStatType.CurrentHealth, maxHealth);
        // Set LocalScale
        sSet.GetValueOrAdd(EntityStatType.LocalScale, 1f);
        sSet.GetValueOrAdd(EntityStatType.Speed, 2f);

        return sSet;
    }

    public virtual void TakeDamage(IoperationStrategy<EntityStatType> operation) {
        if (isInvincible) return;
        float duration = CurrentStats.GetValueOrDefault(EntityStatType.InvincibilityDuration, 0f);

        if (tag != "Player")
            if (duration > 0) {
                isInvincible = true;
                flasher?.Trigger(StatEffectType.Damage, duration, () => isInvincible = false);
            } else {
                flasher?.Trigger(StatEffectType.Damage, 0.2f);
            }
        else {
            StartCoroutine("BecomeInvincible");
        }
            statBroker.UpdateBaseStat(operation);
    }

    IEnumerator BecomeInvincible () {
        isInvincible = true;
        yield return new WaitForSeconds(CurrentStats[EntityStatType.InvincibilityDuration].value);
        isInvincible = false;
    }

    protected virtual void OnStatUpdated() {
        if (CurrentStats[EntityStatType.CurrentHealth].value <= 0) Die();
    }

    public bool AddStatModifier(StatModData def) {
        bool success = false;

        var eMods = def.statMods.FindAll(a => a.GetModCapabilities == ModCapabilities.Entity);

        if (eMods.Count > 0) {
            var eStatMod = new StatModifier<EntityStatType>(eMods, def.GUID, def.duration);
            success = statBroker.Add(eStatMod, def.settable);
        }

        if (success)
            OnAddStatModifier?.Invoke(def);

        return success;
    }

    protected virtual void Die() {
        flasher.OnDestroy();
        //Debug.Log(gameObject.name + " Died");
        //TODO: GAME OVER SCREEN TRIGGER

        Destroy(gameObject);
    }
}

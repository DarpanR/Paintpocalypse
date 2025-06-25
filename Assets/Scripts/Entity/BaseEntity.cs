using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEntity : MonoBehaviour, IVisitor, IstatSetTarget, IWeaponManagerTarget {
    public string entityName;
    [SerializeField, TagMaskField] string target = "Untagged";
    // Only use in Serialize. Never in run time;
    [SerializeField]
    public StatSet stats;
    public List<WeaponDefinition> weapons;


    [Header("Visual Settings")]
    public SpriteRenderer rend;
    public float statFlashSpeed = 0.1f;
    public Color dmgColor = Color.red;

    public event Action OnTakeDamage;
    //public event Action<int> OnUpgrade;
    public event Action<BaseEntity> OnDie;

    StatBroker statBroker;
    WeaponManager weaponManager;

    bool isInvincible = false;

    public int CurrentHealth { get; private set; }
    public StatSet Stats => statBroker.CurrentStats;
    public StatBroker StatBroker => statBroker;
    public WeaponManager WeaponManager => weaponManager;

    protected virtual void Start() {
        statBroker = new StatBroker(stats);
        statBroker.UpdateStats += OnStatUpdated;
        
        CurrentHealth = (int)Stats[StatType.MaxHealth].value;

        weaponManager = new WeaponManager(this.transform, target, weapons);

        if (rend == null) rend = GetComponent<SpriteRenderer>();
    }

    protected virtual void LateUpdate() {
        statBroker.Tick(Time.deltaTime);
    }

    public virtual void TakeDamage(int amount) {
        if (isInvincible)
            return;
        CurrentHealth -= amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, (int)stats[StatType.MaxHealth].value);

        if (CurrentHealth > 0) {
            isInvincible = true;
            StartCoroutine(FlashAndInvincibility());
        } else {
            Die();
        }
        OnTakeDamage?.Invoke();
    }

    protected IEnumerator FlashAndInvincibility() { 
        Color originalColor = rend.color;
        CountdownTimer timer = new CountdownTimer(stats[StatType.InvincibitilityDuration].value);
        timer.Start();

        while (!timer.IsFinished) {
            rend.color = dmgColor;
            yield return new WaitForSeconds(statFlashSpeed);
            rend.color = originalColor;
            yield return new WaitForSeconds(statFlashSpeed);
            timer.Tick(statFlashSpeed * 2);
        }
        rend.color = originalColor;
        isInvincible = false;
    }

    protected virtual void OnStatUpdated(StatSet newStats) {
        stats = newStats;

        var newValue = stats[StatType.LocalScale].value;
        transform.localScale = new Vector3(newValue, newValue);

        newValue = stats[StatType.MaxHealth].value;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, (int)newValue);
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

    public void AddStatModifier(StatModifier modifier) => statBroker.Add(modifier);

    public void Visit(IVisitable visitable) => visitable.Accept(this);
}

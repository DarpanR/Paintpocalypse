using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEntity : MonoBehaviour {
    public string entityName;
    public float moveSpeed = 2f;

    [Header("Health Settings")]
    public int maxHealth = 100;
    [HideInInspector]public int CurrentHealth { get; private set; }

    [Header("Status Settings")]
    public float invincibitilityDuration = 1.0f;
    private bool isInvincible = false;

    [Header("Visual Settings")]
    public SpriteRenderer rend;
    public float statFlashSpeed = 0.1f;
    public Color dmgColor = Color.red;

    List<StatModifier> activeModifiers = new();

    public event Action OnTakeDamage;
    //public event Action<int> OnUpgrade;
    //public event Action OnDie;

    protected virtual void Start() {
        CurrentHealth = maxHealth;
          
        if (rend == null) rend = GetComponent<SpriteRenderer>();
    }

    void LateUpdate() {
        for (int i = activeModifiers.Count - 1; i >= 0; i--) {
            var modifier = activeModifiers[i];
            modifier.lifetime -= Time.deltaTime;

            if (modifier.lifetime <= 0) {
                modifier.Deactivate();
                activeModifiers.RemoveAt(i);
            }
        }
    }

    public virtual void TakeDamage(int amount) {
        if (isInvincible)
            return;
        CurrentHealth -= amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);

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
        float timer = 0f;

        while (timer < invincibitilityDuration) {
            rend.color = dmgColor;
            yield return new WaitForSeconds(statFlashSpeed);
            rend.color = originalColor;
            yield return new WaitForSeconds(statFlashSpeed);
            timer += statFlashSpeed * 2f;
        }
        rend.color = originalColor;
        isInvincible = false;
    }

    public bool AddStatModifier(StatModifier newStatMod) {
        if (activeModifiers.Exists(sm => sm.Definition == newStatMod.Definition))
            return false;
        activeModifiers.Add(newStatMod);
        newStatMod.Activate(this);
        return true;
    }

    protected virtual void Die() {
        Debug.Log(gameObject.name + " Died");
        //TODO: GAME OVER SCREEN TRIGGER
        Destroy(gameObject);
    }

    internal abstract void AddStatModifier(MagnifyingGlass magnifyingGlass);
}

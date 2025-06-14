using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEntity : MonoBehaviour {
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

    public event Action OnTakeDamage;
    //public event Action<int> OnUpgrade;
    //public event Action OnDie;

    protected virtual void Start() {
        CurrentHealth = maxHealth;
          
        if (rend == null) rend = GetComponent<SpriteRenderer>();
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

    protected virtual void Upgrade() {

    }

    protected virtual void Die() {
        Debug.Log(gameObject.name + " Died");
        //TODO: GAME OVER SCREEN TRIGGER
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEntity : MonoBehaviour {
    [Header("Health Settings")]
    public float maxHealth = 100f;
    protected float currentHealth;

    [Header("Status Settings")]
    public float invincibitilityDuration = 1.0f;
    private bool isInvincible = false;

    SpriteRenderer rend;
    public float statFlashSpeed = 0.1f;

    public Color dmgColor = Color.red;

    public float GetHealth () { return currentHealth; }

    protected virtual void Start() {
        currentHealth = maxHealth;
        rend = GetComponent<SpriteRenderer>();
    }

    public virtual void TakeDamage(float amount) {
        if (isInvincible) {
            return;
        }
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth > 0) {
            isInvincible = true;
            StartCoroutine(FlashAndInvincibility());
        } else {
            Die();
        }
    }

    private IEnumerator FlashAndInvincibility() { 
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

    protected virtual void Die() {
        Debug.Log(gameObject.name + " Died");
        //TODO: GAME OVER SCREEN TRIGGER
        Destroy(gameObject);
    }
}

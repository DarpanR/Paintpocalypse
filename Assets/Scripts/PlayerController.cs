using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerController : BaseEntity {
    public float moveSpeed = 5f;
    Rigidbody2D rb;
    Vector2 moveInput;

    public HealthBar healthBar;
    public GameOverOverlay gameOverOverlay;

    // Start is called before the first frame update
    protected override void Start() {
        rb = GetComponent<Rigidbody2D>();
        healthBar.SetMaxHealth(maxHealth);
        base.Start();
    }

    // Update is called once per frame
    void Update() {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

    }

    void FixedUpdate() {
        rb.velocity = moveInput * moveSpeed;
    }

    public override void TakeDamage(int amount)
    {
        if (isInvincible && canBeInvincible)
        {
            return;
        }

        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        healthBar.SetHealth(currentHealth); // Added healthBar update override

        if (currentHealth > 0)
        {
            isInvincible = true;
            StartCoroutine(FlashAndInvincibility());
        }
        else
        {
            Die();
        }
    }
    protected override void Die()
    {
        int score = 0;                      //   PLACEHOLDER, IF WE WANT SCORE NEED CHANGE

        Debug.Log(gameObject.name + " Died");
        Destroy(gameObject);

        gameOverOverlay.Setup(score);
    }
}

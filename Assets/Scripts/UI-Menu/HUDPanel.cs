using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDPanel : MonoBehaviour
{
    [Header("UI Elements")]
    // playerhealth bar
    public Slider healthSlider;
    // Global Game Duration
    public TextMeshProUGUI timerText;
    float globalTimer;

    public void Init(float totalDuration, int maxHealth, int CurrentHealth) {
        // Handle UI
        globalTimer = totalDuration;
        SetMaxHealth(maxHealth);
        SetHealth(CurrentHealth);
    }

    void Update() {
        if (globalTimer > 0)
            globalTimer -= Time.deltaTime; // Decrease timer by the time passed since last frame
        else if (globalTimer < 0)
            globalTimer = 0;

        int minutes = Mathf.FloorToInt(globalTimer / 60);
        int seconds = Mathf.FloorToInt(globalTimer % 60);

        if (globalTimer <= 60)        // Change color to red when timer is below 60s
            timerText.color = Color.red;

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void SetMaxHealth(int maxHealth) {
        healthSlider.maxValue = maxHealth;
    }

    public void SetHealth(int health) {
        healthSlider.value = health;
    }
}

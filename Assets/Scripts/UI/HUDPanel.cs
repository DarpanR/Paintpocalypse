using JetBrains.Annotations;
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

    // Stick Reference
    PlayerController stickman;

    private void Start() {
        globalTimer = PhaseManager.Instance.totalDuration;
        stickman = PlayerController.Instance;

        SetMaxHealth();
        SetHealth();

        stickman.OnTakeDamage += SetHealth;
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

    public void SetMaxHealth() {
        healthSlider.maxValue = stickman.CurrentStats[StatType.MaxHealth].value;
    }

    public void SetHealth() {
        healthSlider.value = stickman.CurrentStats[StatType.CurrentHealth].value;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
    public float fadeDelay;
    public float fadeTime;

    Slider healthSlider;
    CountdownTimer fadeTimer;
    Coroutine fadeCoroutine;

    CanvasGroup cGroup;
    // Start is called before the first frame update

    private void Awake() {
        healthSlider = GetComponent<Slider>();
        cGroup = GetComponent<CanvasGroup>();
        fadeTimer = new CountdownTimer(fadeTime);
    }

    private void OnEnable() {
        GameEvents.OnHealthBarUpdate += UpdateHealth;
    }

    private void OnDisable() {
        GameEvents.OnHealthBarUpdate -= UpdateHealth;
    }

    void UpdateHealth(int health, int maxHealth) {
        healthSlider.value = health;
        healthSlider.maxValue = maxHealth;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        cGroup.alpha = 1.0f;

        fadeTimer.Start();
        fadeCoroutine = StartCoroutine(FadeAfterDelay());
    }

    IEnumerator FadeAfterDelay() {
        yield return new WaitForSeconds(fadeDelay);

        while (fadeTimer.IsRunning) {
            fadeTimer.Tick(Time.deltaTime);
            cGroup.alpha = Mathf.SmoothStep(1f, 0f, fadeTimer.Progress);
            yield return null;
        }
        cGroup.alpha = 0;
    }
}

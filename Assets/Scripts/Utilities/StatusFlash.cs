using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class StatusFlasher : MonoBehaviour {
    [SerializeField]
    StatusEffectDefinition definition;

    public SpriteRenderer rend;
    Dictionary<StatusEffectType, Coroutine> activeEffects = new();
    Dictionary<StatusEffectType, StatusFlashEffect> effectMap = new();

    void Awake() {
        foreach (var flash in definition.flashEffects) {
            if (!effectMap.ContainsKey(flash.type)) {
                effectMap[flash.type] = flash;
            }
        }
    }

    public void Trigger(StatusEffectType type, float duration, Action onEnd = null) {
        if (!effectMap.TryGetValue(type, out StatusFlashEffect effect)) return;
        if (activeEffects.ContainsKey(type)) return;
        Coroutine routine = StartCoroutine(FlashRoutine(effect, duration, onEnd));
        activeEffects[type] = routine;
    }


    IEnumerator FlashRoutine(StatusFlashEffect effect, float duration, Action onEnd = null) {
        Color original = rend.color;
        CountdownTimer timer = new CountdownTimer(duration);
        timer.Start();

        while (!timer.IsFinished) {
            rend.color = effect.color;
            yield return new WaitForSeconds(effect.flashSpeed);
            rend.color = original;
            yield return new WaitForSeconds(effect.flashSpeed);
            timer.Tick(effect.flashSpeed * 2);
        }
        activeEffects.Remove(effect.type);
        rend.color = original;

        onEnd?.Invoke();
    }
}

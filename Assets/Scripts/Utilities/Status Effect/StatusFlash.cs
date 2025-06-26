using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

[RequireComponent(typeof(SpriteRenderer))]
public class StatusFlasher : MonoBehaviour {
    [SerializeField]
    StatusFlasherDefinition allEffects;

    Dictionary<StatusEffectType, Coroutine> activeEffects = new();
    Dictionary<StatusEffectType, StatusFlashEffect> effectMap = new();

    private void Awake() {
        if (allEffects != null) Init(allEffects);
    }

    public void Init (List<StatusFlashEffect> effects) {
        foreach(var effect in effects) 
            if (!effectMap.ContainsKey(effect.type)) 
                effectMap[effect.type] = effect;
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

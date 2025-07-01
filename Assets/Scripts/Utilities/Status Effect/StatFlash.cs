using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatFlasher : MonoBehaviour {
    [SerializeField]
    StatFlashdata allEffects;
    [SerializeField]
    SpriteRenderer rend;

    Dictionary<StatEffectType, Coroutine> activeAbilityEffects = new();
    Dictionary<StatEffectType, StatFlashEffect> effectMap = new();

    private void Awake() {
        rend = rend != null ? rend : GetComponent<SpriteRenderer>();
        if (allEffects != null) Init();
    }

    public void Init() {
        Init(allEffects.AllEffects);
    }

    public void Init (List<StatFlashEffect> effects) {
        foreach (var effect in effects) 
            if (!effectMap.ContainsKey(effect.type)) 
                effectMap[effect.type] = effect;
    }

    public void Trigger(StatEffectType type, float duration, Action onEnd = null) {
        if (activeAbilityEffects.ContainsKey(type) ||
            !effectMap.TryGetValue(type, out StatFlashEffect effect))
            return;

        if (rend != null) {
            Coroutine routine = StartCoroutine(FlashRoutine(effect, duration, onEnd));
            activeAbilityEffects[type] = routine;
        }
    }

    IEnumerator FlashRoutine(StatFlashEffect effect, float duration, Action onEnd = null) {
        Color original = rend.color;
        CountdownTimer timer = new(duration);
        timer.Start();

        while (!timer.IsFinished) {
            rend.color = effect.color;

            yield return new WaitForSeconds(effect.flashSpeed);
            rend.color = original;

            yield return new WaitForSeconds(effect.flashSpeed);
            timer.Tick(effect.flashSpeed * 2);
        }
        activeAbilityEffects.Remove(effect.type);
        rend.color = original;

        onEnd?.Invoke();
    }

    public void OnDestroy() {
        StopAllCoroutines();
    }
}

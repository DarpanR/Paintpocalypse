using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatFlasher : MonoBehaviour {
    [SerializeField]
    StatFlashDefinition allEffects;
    [SerializeField]
    SpriteRenderer rend;

    Dictionary<StatEffectType, Coroutine> activeEffects = new();
    Dictionary<StatEffectType, StatFlashEffect> effectMap = new();

    private void Awake() {
        rend = GetComponent<SpriteRenderer>();
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
        if (activeEffects.ContainsKey(type) ||
            !effectMap.TryGetValue(type, out StatFlashEffect effect))
            return;
        if (rend != null) {
            Coroutine routine = StartCoroutine(FlashRoutine(effect, duration, onEnd));
            activeEffects[type] = routine;
        }
    }

    IEnumerator FlashRoutine(StatFlashEffect effect, float duration, Action onEnd = null) {
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

    public void OnDestroy() {
        StopAllCoroutines();
    }
}

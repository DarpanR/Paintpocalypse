using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(SpriteRenderer))]
public class ModifierDrop : MonoBehaviour {
    [Header("Splash Setting")]
    public Vector2 splashZone;
    public float duration;

    bool dropped = false;
    SpriteRenderer sr;
    StatModifier mod;

    private void Awake() {
         sr = GetComponent<SpriteRenderer>();
    }

    public void Init(StatModifier modifier) {
        sr.sprite = modifier.def.icon;
        mod = modifier;
    }

    private void Update() {
        if (!dropped) return;
        duration -= Time.deltaTime;
        
        if (duration <= 0)
            Destroy(gameObject);
    }

    public void Dropped() {
        CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;

        sr.sprite = mod.def.splashIcon;
        transform.localScale *= splashZone;

        dropped = true;
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (!collision.CompareTag(mod.def.target)) return;
        StatModifier instance = ModifierFactory.Clone(mod);
        collision.GetComponent<BaseEntity>().AddBuff(instance);
    }
}

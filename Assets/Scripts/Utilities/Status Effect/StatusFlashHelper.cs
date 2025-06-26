using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusEffectType {
    None,
    Damage,
    Poison,
    Curse,
    Weakness,
    Buff,
    Freeze
}

[Serializable]
public class StatusFlashEffect {
    public StatusEffectType type;
    public Color color;
    public float flashSpeed;
}
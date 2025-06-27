using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatEffectType {
    None,
    Damage,
    Poison,
    Curse,
    Weakness,
    Buff,
    Freeze
}

[Serializable]
public class StatFlashEffect {
    public StatEffectType type;
    public Color color;
    public float flashSpeed;
}
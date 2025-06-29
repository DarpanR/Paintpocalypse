using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MagnifyingGlass : StatModifier
{
    float sizeMultiplier;
    float speedMultplier;
    float damageMultiplier;


    public MagnifyingGlass(string GUID, float duration, float size, float speed, float damage)
        : base(GUID, duration) {
        sizeMultiplier = size;
        speedMultplier = speed;
        damageMultiplier = damage;
    }

    public override List<IoperationStrategy> Activate() {
        return new() {
            new MultiplyOperation(StatType.LocalScale, sizeMultiplier),
            new MultiplyOperation(StatType.Speed,  speedMultplier),
            new MultiplyOperation( StatType.Damage, damageMultiplier)
        };
    }
}

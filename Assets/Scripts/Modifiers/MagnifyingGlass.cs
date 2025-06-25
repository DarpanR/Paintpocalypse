using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MagnifyingGlass : StatModifier, IStatSetModifier, IWeaponModifier
{
    public MultiplyOperation speedOperation;
    public MultiplyOperation damageOperation;
    public MultiplyOperation sizeOperation;

    public MagnifyingGlass(ModifierDefinition definition) : base(definition) {
        MagnifyingGlassDefinition def = definition as MagnifyingGlassDefinition;

        speedOperation = new MultiplyOperation(def.speedMultplier);
        damageOperation = new MultiplyOperation(def.damageMultiplier);
        sizeOperation = new MultiplyOperation(def.sizeMultiplier);
    }

    public void Activate(StatSet stats) {
        stats[StatType.MoveSpeed].Apply(speedOperation);
        stats[StatType.LocalScale].Apply(speedOperation);
    }

    public void Activate(WeaponManager weaponManager) {
        foreach(var weapon in weaponManager.allWeapons) {
            if (weapon is IstatSetTarget statTarget) 
                statTarget.StatBroker.Add(this);
        }
    }
}

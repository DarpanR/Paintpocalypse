using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MagnifyingGlass : StatModifier, IWeaponModifier
{
    public MagnifyingGlass(ModifierDefinition definition) : base(definition) {}

    public void Activate(WeaponManager weaponManager) {
        foreach(var weapon in weaponManager.allWeapons) {
            if (weapon is IstatSetTarget statTarget) 
                statTarget.StatBroker.Add(this);
        }
    }

    public override List<IoperationStrategy> Activate() {
        MagnifyingGlassDefinition def = Definition as MagnifyingGlassDefinition;

        return new() {
            new MultiplyOperation(StatType.LocalScale, def.sizeMultiplier),
            new MultiplyOperation(StatType.MoveSpeed,  def.speedMultplier),
            new MultiplyOperation( StatType.Damage, def.damageMultiplier)
        };
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MagnifyingGlass : StatModifier, IWeaponModifier
{
    public MagnifyingGlass(ModifierDefinition definition) : base(definition) {}

    public void Activate(WeaponManager weaponManager) {
        foreach (var weapon in weaponManager.Weapons)
            if (weapon is IstatSetTarget statTarget)
                statTarget.AddStatModifier(this);
    }

    public override List<IoperationStrategy> Activate() {
        MagnifyingGlassDefinition def = Definition as MagnifyingGlassDefinition;

        return new() {
            new MultiplyOperation(StatType.LocalScale, def.sizeMultiplier),
            new MultiplyOperation(StatType.Velocity,  def.speedMultplier),
            new MultiplyOperation( StatType.Damage, def.damageMultiplier)
        };
    }
}

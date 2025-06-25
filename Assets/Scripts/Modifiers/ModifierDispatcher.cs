using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ModifierDispatcher {
    public static void ApplyModifier(GameObject go, StatModifier modifier) {
        if (modifier is IStatSetModifier statMod && 
            go.TryGetComponent(out IstatSetTarget statTarget)) {
            statMod.Activate(statTarget.Stats);
        }

        if (modifier is IWeaponModifier weaponMod && 
            go.TryGetComponent(out IWeaponManagerTarget weaponTarget)) {
            weaponMod.Activate(weaponTarget.WeaponManager);
        }
    }
}

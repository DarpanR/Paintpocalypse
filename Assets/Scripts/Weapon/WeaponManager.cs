using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class WeaponManager {
    string targetTag;
    Transform firePoint;
    Dictionary<WeaponDefinition, IWeaponModule> weapons = new();

    public IEnumerable<IWeaponModule> Weapons => weapons.Values;

    public WeaponManager(Transform firePoint, List<WeaponDefinition> allWeapons, string targetTag) {
        this.firePoint = firePoint;
        this.targetTag = targetTag;

        foreach (var weapon in allWeapons)
            Equip(weapon);
    }

    public void Update() {
        // each weapon shoots at its own fire-rate
        foreach (var weapon in weapons.Values) {
            weapon.TryFire();
        }
    }

    /// <summary>
    /// Call this when the player picks up a weapon drop or levels up.
    /// </summary>
    public void Equip(WeaponDefinition def) {
        if(weapons.TryGetValue(def, out var existing))
            existing.Upgrade();
        else {
            var module = def.CreateModule(firePoint, targetTag);
            weapons[def]= module;   
        }
        // TODO: notify HUD with new weapon icon
    }
}

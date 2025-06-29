using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class WeaponManager {
    string targetTag;
    Transform firePoint;
    MonoBehaviour runner;
    Dictionary<WeaponData, IWeaponModule> weapons = new();

    public IEnumerable<IWeaponModule> Weapons => weapons.Values;

    public WeaponManager(Transform firePoint, List<WeaponData> allWeapons, string targetTag, MonoBehaviour runner) {
        this.firePoint = firePoint;
        this.runner = runner;
        this.targetTag = targetTag;

        foreach (var weapon in allWeapons)
            Equip(weapon);
    }

    public void Update() {
        // each weapon shoots at its own fire-rate
        foreach (var weapon in weapons.Values)
            weapon.TryFire();
    }

    /// <summary>
    /// Call this when the player picks up a weapon drop or levels up.
    /// </summary>
    public void Equip(WeaponData def) {
        if(weapons.TryGetValue(def, out var existing))
            existing.Upgrade();
        else {
            var module = def.CreateModule(firePoint, runner, targetTag);
            weapons[def]= module;   
        }
        // TODO: notify HUD with new weapon icon
    }
}

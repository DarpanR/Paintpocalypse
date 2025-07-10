using System.Collections.Generic;
using UnityEngine;

public class WeaponManager {
    readonly string targetTag;

    Transform parentOrigin;
    Dictionary<WeaponData, WeaponModule> weapons = new();

    public IEnumerable<WeaponModule> Weapons => weapons.Values;

    public WeaponManager(Transform parentOrigin, List<WeaponData> allWeapons, string targetTag) {
        this.parentOrigin = parentOrigin;
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
    /// Call this when the target picks up a weapon drop or levels up.
    /// </summary>
    public bool Equip(WeaponData weaponData) {
        if(weapons.TryGetValue(weaponData, out var existing))
            return existing.Upgrade();
        var module = new WeaponModule(parentOrigin, weaponData, targetTag);
        weapons[weaponData] = module;   
        return true;
        // TODO: notify HUD with new weapon icon
    }
}

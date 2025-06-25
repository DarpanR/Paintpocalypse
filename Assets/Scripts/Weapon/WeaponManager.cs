using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponManager {
    [Header("Where bullets originate")]
    public Transform firePoint;

    // all the list of SO defintion
    public List<WeaponDefinition> allWeapons = new List<WeaponDefinition>();

    List<IWeaponModule> weapons = new();
    string target;

    public WeaponManager(Transform firePoint, string target, List<WeaponDefinition> weapons) {
        this.firePoint = firePoint;
        this.target = target;
        allWeapons = weapons;

        foreach (var weapon in allWeapons)
            Equip(weapon);
    }

    private void Update() {
        // each weapon shoots at its own fire-rate
        foreach(var weapon in weapons) {
            weapon.TryFire();
        }
    }

    /// <summary>
    /// Call this when the player picks up a weapon drop or levels up.
    /// </summary>
    public void Equip(WeaponDefinition def) {
        // find existing
        var existing = weapons.FirstOrDefault(m => m.Definition == def);

        if (existing != null)
            existing.Upgrade();
        else
            weapons.Add(def.CreateModule(firePoint, target));
        // TODO: notify HUD with new weapon icon
    }
}

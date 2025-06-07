using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponManager : MonoBehaviour {
    public List<BaseWeapon> weapons = new();

    public void AddWeapon(BaseWeapon newWeapon) {
        if (weapons.Contains(newWeapon)) {
            weapons.Find(weapon => weapon == newWeapon).LevelUp();
        } else {
            weapons.Add(newWeapon);
        }
    }

    private void Update() {
        foreach (var weapon in weapons) {
            weapon.TryFire(transform.position);
        }
    }
}
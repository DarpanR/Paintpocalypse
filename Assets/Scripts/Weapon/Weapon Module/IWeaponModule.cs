using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeaponModule
{
    /// <summary>
    /// The static data this module was built from.
    /// (always non?null)
    /// </summary>
    WeaponData data { get; }

    /// <summary>
    /// Called every frame by WeaponManager to handle cooldown & firing.
    /// </summary>
    void TryFire();

    /// <summary>
    /// Called when the player picks up the same weapon (or levels up).
    /// </summary>
    void Upgrade();
}

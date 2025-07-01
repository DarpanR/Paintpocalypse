using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeaponModule
{
    /// <summary>
    /// Called every frame by WeaponManager to handle cooldown & firing.
    /// </summary>
    void TryFire();

    /// <summary>
    /// Called when the target picks up the same weapon (or levels up).
    /// </summary>
    bool Upgrade();
}

using System;
using UnityEngine;

public class WeaponPickup : PickupHandler {

    [SerializeField]
    WeaponData data;

    public override IPickupData Data => data as IPickupData;

    protected override void Awake() {
        if (data != null && data is not IPickupData)
            Debug.LogWarning($"{name}'s StatModData is not Ipickupdata!");
        PickupType = PickupType.Weapon;
        base.Awake();
    }

    public override void Init(IPickupData pickupData, bool dropIt = false) {
        data = data != null ? data : pickupData as WeaponData;
        base.Init(pickupData, dropIt);
    }
}

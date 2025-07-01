using System;
using UnityEngine;

public class ModifierPickup : PickupHandler
{
    [SerializeField]
    StatModData data;

    public override IPickupData Data => data as IPickupData;

    protected override void Awake()
    {
        if (data != null && data is not IPickupData)
            Debug.LogWarning($"{name}'s StatModData is not Ipickupdata!");
        PickupType = PickupType.StatModifier;
        base.Awake();
    }

    public override void Init(IPickupData pickupData, bool dropIt = false) {
        data = data != null ? data : pickupData as StatModData;
        base.Init(pickupData, dropIt);
    }
}
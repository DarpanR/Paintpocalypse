using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

    public override void Init(IPickupData data, bool dropIt = false) {
        this.data = this.data ?? data as StatModData;
        base.Init(data, dropIt);
    }
}
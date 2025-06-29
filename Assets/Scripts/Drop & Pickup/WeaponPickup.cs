using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
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

    public override void Init(IPickupData data, bool dropIt = false) {
        this.data = this.data ?? data as WeaponData;
        base.Init(data, dropIt);
    }
}

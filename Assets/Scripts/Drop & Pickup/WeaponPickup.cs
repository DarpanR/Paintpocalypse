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
    WeaponDefinition definition;

    protected override IPickupDefinition Definition => definition; 

    protected override void Awake()
    {
       PickupType = PickupType.Weapon;
    }
    protected override void PickUp(BaseEntity entity) {
        if (entity.CompareTag(definition.pickupTag)) {
            entity.WeaponManager.Equip(definition);
            Destroy(gameObject);
        }
    }
}

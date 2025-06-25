using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class WeaponPickup : PickupHandler {
    protected override void PickUp(BaseEntity entity) {
        if (entity.CompareTag("Player") &&
            entity.TryGetComponent(out WeaponManager wm)) {
            wm.Equip((WeaponDefinition)Definition);
            Destroy(gameObject);
        }
    }
}

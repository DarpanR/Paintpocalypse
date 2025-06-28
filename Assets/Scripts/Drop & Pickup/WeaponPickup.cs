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

    protected override IPickupDefinition Definition => definition as IPickupDefinition;

    protected override void Awake() {
        if (definition != null && definition is not IPickupDefinition)
            Debug.LogWarning($"{name}'s ModifierDefinition is not IpickupDefinition!");
        PickupType = PickupType.Weapon;
        base.Awake();
    }

    public override void Init(IPickupDefinition definition, bool dropIt = false) {
        this.definition = this.definition ?? definition as WeaponDefinition;
        base.Init(definition, dropIt);
    }

    protected override void PickUp(BaseEntity entity) {
        if (entity == null) return;
        remainingUsage--;
        entity.WeaponManager.Equip(definition);

        base.PickUp(entity);
    }
}

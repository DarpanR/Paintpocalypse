using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierPickup : PickupHandler
{
    [SerializeField]
    ModifierDefinition definition;

    protected override IPickupDefinition Definition => definition as IPickupDefinition;

    protected override void Awake()
    {
        PickupType = PickupType.StatModifier;
    }

    protected override void PickUp(BaseEntity entity) { 
        if (!entity.CompareTag(Definition.PickupTag)) return;
        var modifier = definition.CreateModule(definition);

        if (entity.AddStatModifier(modifier) && --remainingUsage <= 0) { 
            Debug.Log(entity.name + " Picked up " + definition.modName);
            Destroy(gameObject);
        }
    }
}
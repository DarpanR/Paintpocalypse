using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierPickup : PickupHandler
{
    [SerializeField]
    ModifierDefinition definition;

    protected override IPickupDefinition Definition => definition;

    protected override void PickUp(BaseEntity entity) { 
        if (!entity.CompareTag(definition.target)) return;
        var modifier = definition.CreateModule(definition);

        if (entity.StatBroker.Add(modifier) && --remainingUsage <= 0) { 
            Debug.Log(entity.name + " Picked up " + definition.modName);
            Destroy(gameObject);
        }
    }
}
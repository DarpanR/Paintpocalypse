using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ModifierPickup : PickupHandler
{
    [SerializeField]
    ModifierDefinition definition;

    protected override IPickupDefinition Definition => definition as IPickupDefinition;

    protected override void Awake()
    {
        if (definition != null && definition is not IPickupDefinition)
            Debug.LogWarning($"{name}'s ModifierDefinition is not IpickupDefinition!");
        PickupType = PickupType.StatModifier;
        base.Awake();
    }

    public override void Init(IPickupDefinition definition, bool dropIt = false) {
        this.definition = this.definition ?? definition as ModifierDefinition;
        base.Init(definition, dropIt);
    }

    protected override void PickUp(BaseEntity entity) { 
        //if(definition == null) {
        //    Debug.LogError($"{name} tried to pick up, but has no definition!");
        //    return;
        //}
        //if (!entity.CompareTag(Definition.PickupTag)) return;
        var modifier = definition.CreateModule(definition);

        if (entity.AddStatModifier(modifier)) {
            remainingUsage--;
            Debug.Log(entity.name + " Picked up " + definition.modName);
        }
        base.PickUp(entity);
    }
}
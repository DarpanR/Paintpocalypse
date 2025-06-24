using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedModifier : StatModifier
{
    public SpeedModifier(ModifierDefinition _def) : base(_def) {
    }

    public override void Add(BaseEntity entity) {
        float moveSpeed = Mathf.Round(entity.moveSpeed * def.modifier);
        Debug.Log(moveSpeed); 
        entity.moveSpeed = moveSpeed;
    }

    public override void Remove(BaseEntity entity) {
        entity.moveSpeed = Mathf.Round(entity.moveSpeed / def.modifier);
    }
}
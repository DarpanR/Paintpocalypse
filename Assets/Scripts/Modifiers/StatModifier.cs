using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatModifier
{
    public ModifierDefinition def;
    public float lifetime;
    
    public StatModifier(ModifierDefinition _def) {
        def = _def;
        lifetime = def.duration;
    }

    public abstract void Add(BaseEntity entity);
    public abstract void Remove(BaseEntity entity); 
}

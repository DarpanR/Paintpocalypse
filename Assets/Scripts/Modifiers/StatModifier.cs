using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatModifier<Tdef> where Tdef: ModifierDefintion {
    
    public Tdef Definition { get; private set; }
    public float lifetime;

    public StatModifier(Tdef definition) {
        Definition = definition;
        lifetime = definition.duration;
    }

    public abstract void Activate(BaseEntity entity);
    public abstract void Deactivate();
}

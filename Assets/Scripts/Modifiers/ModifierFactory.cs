using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ModifierTypes { Speed, Size, Damage }
public static class ModifierFactory {
    public static StatModifier CreateModifier(ModifierDefinition definition) {
        switch (definition.type) {
            case ModifierTypes.Speed:
                return new SpeedModifier(definition);
            //case ModifierTypes.Size:
            //return new SizeModifier(definition);
            //case ModifierTypes.Damage:
            //return new DamageModifier(definition);
            default:
                Debug.Log("Unknow modifier type.");
                return null;
        }
    }

    public static StatModifier Clone(StatModifier modifier) {
        return CreateModifier(modifier.def);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Custom/Stat Modifier/Magnifying Glass")]
public class MagnifyingGlassDefinition : ModifierDefinition
{
    public float sizeMultiplier = 2f;
    public float damageMultiplier = 2f;
    public float speedMultplier = 2f;

    public override StatModifier CreateModule(ModifierDefinition definition) {
        return new MagnifyingGlass(definition);
    }
}

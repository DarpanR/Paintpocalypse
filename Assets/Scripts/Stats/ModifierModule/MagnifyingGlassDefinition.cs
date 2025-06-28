using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Custom/Stat Modifier/Magnifying Glass")]
public class MagnifyingGlassDefinition : ModifierDefinition, IPickupDefinition
{
    [Header("Pickup Settings")]
    public Sprite pickupIcon;
    public Sprite dropIcon;
    [TagMaskField]
    public string pickupTag;
    [Min(1)]
    public int pickupCount = 1;
    public DropType dropType = DropType.Counter;
    [Min(1)]
    public int dropCount = 1;

    [Header("Magnifying Glass Setting")]
    public float sizeMultiplier = 2f;
    public float damageMultiplier = 2f;
    public float speedMultplier = 2f;

    public override StatModifier CreateModule(ModifierDefinition definition) {
        return new MagnifyingGlass(definition);
    }

    public string DisplayName => modName;
    public Sprite PickupIcon => pickupIcon;
    public Sprite DropIcon => dropIcon;
    public string PickupTag => pickupTag;
    public PickupType PickupType => PickupType.StatModifier;
    public DropType DropType => dropType;
    public int DropCount => dropCount;
    public float PickupCount => pickupCount;
}

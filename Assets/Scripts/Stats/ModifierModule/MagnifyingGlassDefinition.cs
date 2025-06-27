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
    public PickupType pickupType = PickupType.StatModifier;
    [Min(1)]
    public int pickupCount = 1;
    public DropType dropType = DropType.Counter;
    [Min(1)]
    public int dropCount = 1;

    public float sizeMultiplier = 2f;
    public float damageMultiplier = 2f;
    public float speedMultplier = 2f;

    public override StatModifier CreateModule(ModifierDefinition definition) {
        return new MagnifyingGlass(definition);
    }
    public Sprite PickupIcon => pickupIcon;
    public Sprite DropIcon => dropIcon;
    public string PickupTag => pickupTag;
    public PickupType PickupType => pickupType;
    public DropType DropType => DropType;
    public int DropCount => dropCount;
    public float PickupCount => pickupCount;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Stat Modifier/Magnifying Glass")]
public class MagGlassModData : StatModData, IPickupData {
    [Header("Magnifying Glass Setting")]
    public float sizeMultiplier = 2f;
    public float damageMultiplier = 2f;
    public float speedMultplier = 2f;

    [Header("Pickup Settings")]
    public Sprite pickupIcon;
    public Sprite dropIcon;
    [TagMaskField]
    public string pickupTag;
    [Min(-1)]
    public int totalUsage = 1;
    [Min(-1)]
    public float lifetime = 5f;
    public TargetingType dropOperationType = TargetingType.OverMouse;
    public float dropRadius = 1f;

    protected override ModifierCapabilities GetCapabilities() {
        return ModifierCapabilities.Weapon | ModifierCapabilities.Stat;
    }

    public override StatModifier CreateModule(StatModData data) {
        var def = data as MagGlassModData;
        return new MagnifyingGlass(
            def.GUID,
            def.duration,
            def.sizeMultiplier,
            def.speedMultplier,
            def.damageMultiplier
        );
    }

    public string PickupName => modName;
    public Sprite PickupIcon => pickupIcon;
    public Sprite DropIcon => dropIcon != null ? dropIcon : pickupIcon;
    public string PickupTag => pickupTag;
    public PickupType PickupType => PickupType.StatModifier;
    public float LifeTime => lifetime;
    public int TotalUsage => totalUsage;
    public TargetingType TargetingType => dropOperationType;
    public float TargetRadius => dropRadius;
}

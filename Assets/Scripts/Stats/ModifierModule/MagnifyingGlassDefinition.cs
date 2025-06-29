using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Stat Modifier/Magnifying Glass")]
public class MagGlassModData : StatModData, IPickupData {
    [Header("Pickup Settings")]
    public Sprite pickupIcon;
    public Sprite dropIcon;
    [TagMaskField]
    public string pickupTag;
    [Min(1)]
    public int pickupCount = 1;
    [Min(-1)]
    public int totalUsage = 1;
    [Min(-1)]
    public float lifetime = 5f;
    public DropType dropOperationType = DropType.OverMouse;
    public float dropRadius = 3f;
    public float dropForce = 5f;

    [Header("Magnifying Glass Setting")]
    public float sizeMultiplier = 2f;
    public float damageMultiplier = 2f;
    public float speedMultplier = 2f;

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

    public string DisplayName => modName;
    public Sprite PickupIcon => pickupIcon;
    public Sprite DropIcon => dropIcon;
    public string PickupTag => pickupTag;
    public PickupType PickupType => PickupType.StatModifier;
    public float LifeTime => lifetime;
    public int TotalUsage => totalUsage;
    public DropType DropType => dropOperationType;
    public float PickupCount => pickupCount;

    public float DropRadius => dropRadius;

    public float DropForce => dropForce;
}

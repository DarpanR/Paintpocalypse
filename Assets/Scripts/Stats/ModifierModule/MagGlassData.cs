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
    public PickupData pickupData = new() {
        totalUsage = 1,
        lifeTime = 5f,
        dropOperationType = TargetingType.OverMouse,
        dropRadius = 1f
    };

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
    public Sprite PickupIcon => pickupData.pickupIcon;
    public Sprite DropIcon => pickupData.dropIcon != null ? pickupData.dropIcon : pickupData.pickupIcon;
    public string PickupTag => pickupData.pickupTag;
    public float LifeTime => pickupData.lifeTime;
    public int TotalUsage => pickupData.totalUsage;
    public TargetingType TargetingType => pickupData.dropOperationType;
    public float TargetRadius => pickupData.dropRadius;
    public PickupType PickupType => PickupType.StatModifier;
}

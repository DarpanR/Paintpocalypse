using System;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName ="New Stat Modifier", menuName ="Custom/Stat Modifier")]
public class StatModData : ScriptableObject, IPickupData {
    [Header("Modifier Setting")]
    public string modName;
    public float duration;
    public SettableType settable = SettableType.Single;

    [SerializeReference]
    public List<StatModBehavior> statMods;

    [Header("Pickup Settings")]
    public PickupData pickupData = new() {
        totalUsage = 1,
        lifeTime = 5f,
        dropOperationType = TargetingType.OverMouse,
        dropRadius = 1f
    };

    [SerializeField, HideInInspector]
    string guid = Guid.NewGuid().ToString();

    public string GUID {
        get {
            if (string.IsNullOrEmpty(guid)) 
                guid = Guid.NewGuid().ToString();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
            return guid; 
        }
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
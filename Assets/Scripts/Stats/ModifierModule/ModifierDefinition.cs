using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum settableType { Single, Timer, Multi}

public abstract class ModifierDefinition : ScriptableObject, IPickupDefinition {
    [Header("Pickup Settings")]
    public Sprite pickupIcon;
    public Sprite dropIcon;
    public PickupType pickupType = PickupType.StatModifier;
    [Min(1)]
    public int pickupCount = 1;
    public DropType dropType = DropType.Counter;
    [Min(1)]
    public int dropCount = 1;

    [Header("Modifier Setting")]
    public string modName;
    public float duration;
    public settableType settable = settableType.Single;
    [TagMaskField] 
    public string target = "Untagged";

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

    public Sprite PickupIcon => pickupIcon;
    public Sprite DropIcon => dropIcon;
    public PickupType PickupType => pickupType;
    public DropType DropType => DropType;
    public int DropCount => dropCount;
    public float PickupCount => pickupCount;

    public abstract StatModifier CreateModule(ModifierDefinition definition);
}

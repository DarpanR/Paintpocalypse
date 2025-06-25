using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum settableType { Single, Timer, Multi}

public abstract class ModifierDefinition : ScriptableObject, IPickupDefinition {

    [Header("Pickup Setting")]
    public Sprite pickupIcon;
    public PickupType pickupType = PickupType.StatModifier;
    public int amount = 1;

    [Header("Modifier Setting")]
    public string modName;
    public float duration;
    public settableType settable = settableType.Single;
    [TagMaskField] 
    public string target = "Untagged";

    [SerializeField, HideInInspector]
    public string guid = Guid.NewGuid().ToString();

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

    public PickupType PickupType => pickupType;
    public int Amount => amount;
    public Sprite PickupIcon => pickupIcon;

    public abstract StatModifier CreateModule(ModifierDefinition definition);


}

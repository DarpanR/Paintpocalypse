using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class ModifierDefinition : ScriptableObject {
    [Header("Modifier Setting")]
    public string modName;
    public float duration;
    public settableType settable = settableType.Single;

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

    public abstract StatModifier CreateModule(ModifierDefinition definition);
}

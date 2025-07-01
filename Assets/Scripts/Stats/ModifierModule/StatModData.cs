using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class StatModData : ScriptableObject {
    [Header("Modifier Setting")]
    public string modName;
    public float duration;
    public ModifierCapabilities Capabilities => GetCapabilities();
    public SettableType settable = SettableType.Single;

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

    protected abstract ModifierCapabilities GetCapabilities();

    public abstract StatModifier CreateModule(StatModData data);
}

using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName ="Custom/Entity Data")]
public class EntityData : ScriptableObject
{
    public string entityName;
    [TagMaskField]
    public string targetTag = "Untagged";
    public StatSet<EntityStatType> baseStats = new StatSet<EntityStatType>(
        new Stat<EntityStatType>(EntityStatType.MaxHealth, 50f),
        new Stat<EntityStatType>(EntityStatType.Speed, 2f),
        new Stat<EntityStatType>(EntityStatType.LocalScale, 1f)
    );
    public List<WeaponData> loadOutWeapons;
    public List<StatFlashEffect> VisualStatusEffects = new List<StatFlashEffect> {
        new StatFlashEffect(StatEffectType.Damage, Color.red, 0.1f)
    };
    [SerializeField]
    public EntityDropEntry dropEntry = new EntityDropEntry { 
        Cost = 10,
        Exp = 10,
        weight = 0.5f,
        dropTablesAllowed = new int[] { 0 },
   
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

}

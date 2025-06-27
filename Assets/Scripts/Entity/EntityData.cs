using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Custom/Entity Data")]
public class EntityData : ScriptableObject
{
    public string entityName;
    [TagMaskField]
    public string targetTag;
    public StatSet baseStats;
    public List<WeaponDefinition> loadOutWeapons;
    public List<StatFlashEffect> VisualStatusEffects;
}

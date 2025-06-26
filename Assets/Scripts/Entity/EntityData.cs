using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Custom/Entity Data")]
public class EntityData : ScriptableObject
{
    public string entityName;
    [TagMaskField]
    public string targetTag;
    public StatSet stats;
    public List<WeaponDefinition> allWeapons;
    public List<StatusFlasher> allEffects;
}

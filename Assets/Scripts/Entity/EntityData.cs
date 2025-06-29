using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Custom/Entity Data")]
public class EntityData : ScriptableObject
{
    public string entityName;
    [TagMaskField]
    public string targetTag = "Untagged";
    public StatSet baseStats = new StatSet(
        new Stat(StatType.MaxHealth, 50f),
        new Stat(StatType.Speed, 2f),
        new Stat(StatType.LocalScale, 1f)
    );
    public List<WeaponData> loadOutWeapons;
    public List<StatFlashEffect> VisualStatusEffects = new List<StatFlashEffect> {
        new StatFlashEffect(StatEffectType.Damage, Color.red, 0.1f)
    };
}

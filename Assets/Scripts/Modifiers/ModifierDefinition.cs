using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Custom/Modifer Definition")]
public class ModifierDefintion : ScriptableObject {
    public string StatModifierName;
    public Sprite icon;
    public float duration;
    [TagMaskField] 
    public string target = "Untagged";
    public TargetingMode targetingMode;
}

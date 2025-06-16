using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Custom/Modifier Definition")]
public class ModifierDefinition : ScriptableObject {
    public string modifierName;
    public Sprite icon;
    public Sprite splashIcon;
    public float duration;
    public float modifier;
    public ModifierTypes type;
    [TagMaskField] 
    public string target = "Untagged";
}

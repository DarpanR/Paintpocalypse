using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Custom/Mouse Ability")]
public class MouseAbilityDefintion : ScriptableObject
{
    public string AbilityName;
    public Sprite dragIcon;
    public Sprite dropIcon;
    public TargetingMode targetingMode;

}

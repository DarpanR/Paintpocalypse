using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Custom/Flash Effect Data")]
public class StatFlashDefinition : ScriptableObject
{
    public SpriteRenderer rend;
    public List<StatFlashEffect> AllEffects;
}


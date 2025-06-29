using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Custom/Flash Effect Data")]
public class StatFlashdata : ScriptableObject
{
    public SpriteRenderer rend;
    public List<StatFlashEffect> AllEffects;
}


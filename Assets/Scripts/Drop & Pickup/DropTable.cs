using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Drop Table", menuName = "Custom/Drop Table")]
public class DropTable : ScriptableObject {
    public List<DropEntry> drops;
    public AnimationCurve dropChance;
}

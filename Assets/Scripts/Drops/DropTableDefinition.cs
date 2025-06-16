using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DropEntry {
    public WeaponDefinition weapon;
    public int weight;
}

[CreateAssetMenu(fileName ="Drop Table", menuName = "Custom/Drop Table")]
public class DropTable : ScriptableObject {
    public List<DropEntry> drops;
}
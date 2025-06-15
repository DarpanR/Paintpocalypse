using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnInstruction {
    public GameObject enemy;      // which enemy
    public int minCount = 1;            // how many at minimum
    public int maxCount = 1;            // how many at maximum
    public SpawnPatternType pattern;    // Burst, Radial, Linear…
}

[Serializable]
public class WaveInstruction {
    [Tooltip("Seconds after phase start")]
    public float timeOffSet;
    public List<SpawnInstruction> spawns;
}
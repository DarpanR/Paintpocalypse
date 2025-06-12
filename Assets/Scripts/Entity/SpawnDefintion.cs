using JetBrains.Annotations;
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

[CreateAssetMenu(menuName = "Custom/Spawn Defintion")]
public class PhaseDefinition : ScriptableObject {
    [Tooltip("How long this phase lasts (sec)")]
    public float duration = 30f;
    [Tooltip("Spawn these every interval")]
    public float periodicInterval = 5f;
    public List<SpawnInstruction> periodicSpawns;

    [Tooltip("One-off waves at these times")]
    public List<WaveInstruction> specialWaves;
}

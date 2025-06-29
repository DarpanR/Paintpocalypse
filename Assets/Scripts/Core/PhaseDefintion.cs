using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Phase Defintion")]
public class Phasedata : ScriptableObject {
    [TextArea,Tooltip("Only for organization tracking")]
    public string discription;
    [Tooltip("How long this phase lasts (sec)")]
    public float duration = 30f;
    [Tooltip("Spawn these every interval")]
    public float periodicInterval = 5f;
    public List<SpawnInstruction> periodicSpawns;

    [Tooltip("One-off waves at these times")]
    public List<WaveInstruction> specialWaves;
}

//[CreateAssetMenu(
//    menuName = "Spawns/SpecialSpawn",
//    fileName = "NewSpecialSpawn")]
//public class SpecialSpawn : ScriptableObject {
//    [Tooltip("When this wave spawns (seconds)")]
//    public float startTime;
//    [Tooltip("What to spawn each interval")]
//    public List<SpawnInstruction> spawns;       // all the things to spawn in this wave
//}

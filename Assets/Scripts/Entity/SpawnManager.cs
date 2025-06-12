//using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public enum SpawnPatternType { Burst, Radial, Linear/*, ZigZag */}
public class SpawnManager : MonoBehaviour {

    [Header("Phase Instruction")]
    public List<PhaseDefinition> phases;

    int currentPhase = 0;
    float timer = 0f;
    float nextPeriodic = 0f;
    int nextWave = 0;

    Camera mainCam;
    float radius;

    //Pattern logic
    public int buffer = 1;
    public float spawnRadius = 2f;

    void Start() {
        mainCam = Camera.main;
        // Use the camera’s orthographic size and aspect ratio to compute the edge for some spawn patterns
        radius = Mathf.Max(mainCam.orthographicSize, mainCam.orthographicSize * mainCam.aspect) + buffer;

        foreach (var phase in phases)
            phase.specialWaves.Sort((a, b) => a.timeOffSet.CompareTo(b.timeOffSet));
        BeginPhase(0);
    }

    void Update() {
        timer += Time.deltaTime;
        var phase = phases[currentPhase];

        // Periodic Spawns
        if (timer >= nextPeriodic) {
            FireInstruction(phase.periodicSpawns[Random.Range(0, phase.periodicSpawns.Count)]);
            nextPeriodic += phase.periodicInterval;
        }

        // Special Spawns
        while(nextWave < phase.specialWaves.Count && timer >= phase.specialWaves[nextWave].timeOffSet) {
            foreach (var ins in phase.specialWaves[nextWave].spawns) FireInstruction(ins);
            nextWave++;
        }

        // End of phase?
        if (timer >= phase.duration) {
            int next = currentPhase + 1;

            if (next >= phases.Count) next = 0;
            BeginPhase(next);
        }
    }

    void BeginPhase(int phaseIndex) {
        currentPhase = phaseIndex;
        timer = 0f;
        nextPeriodic = phases[phaseIndex].periodicInterval;
        nextWave = 0;
    }

    void FireInstruction(SpawnInstruction ins) {
        int count = Random.Range(ins.minCount, ins.maxCount + 1);
        List<Vector2> positions = GeneratePattern(ins.pattern, count);

        for (int i = 0; i < count; i++) {
            Instantiate(ins.enemy, positions[i], Quaternion.identity);
        }
    }

    Vector2 GetOffscreenPosition() {
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector2[] edges = new Vector2[] {
            new Vector2(Random.Range(0, screenSize.x), -buffer),               // bottom
            new Vector2(Random.Range(0, screenSize.x), screenSize.y + buffer), // top
            new Vector2(-buffer, Random.Range(0, screenSize.y)),               // left
            new Vector2(screenSize.x + buffer, Random.Range(0, screenSize.y))  // right
       };
        Vector2 screenPoint = edges[Random.Range(0, edges.Length)];
        Vector3 worldPoint = mainCam.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, 0));
        return (Vector2)worldPoint;
    }

    List<Vector2> GeneratePattern(SpawnPatternType pattern, int count) {
        List<Vector2> positions = new List<Vector2>();

        switch (pattern) {
            case SpawnPatternType.Burst: {
                    // Choose offscreen anchor
                    Vector2 anchor = GetOffscreenPosition();

                    for (int i = 0; i < count; i++) {
                        positions.Add(anchor + Random.insideUnitCircle * spawnRadius);
                    }
                    break;
                }
            case SpawnPatternType.Radial: {
                    float angleStep = 360f / count;
                    Vector2 camCenter = (Vector2)mainCam.transform.position;

                    for (int i = 0; i < count; i++) {
                        float angle = angleStep * i * Mathf.Deg2Rad;
                        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                        Vector2 spawnPos = camCenter + offset * radius;
                        positions.Add(spawnPos);
                    }
                    break;
                }
            case SpawnPatternType.Linear: {
                    float angle = Random.Range(0, Mathf.PI * 2);
                    //TODO: Change the spacing to be dynamic
                    float spacing = 2f;

                    Vector2 dirFromPlayer = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                    Vector2 lineCenter = (Vector2)mainCam.transform.position + dirFromPlayer * radius;

                    Vector2 prep = new Vector2(-dirFromPlayer.y, dirFromPlayer.x).normalized;

                    Vector2 lineStart = lineCenter - prep * ((count - 1 * spacing) / 2);

                    for (int i = 0; i < count; i++) {
                        Vector2 spawnPos = lineStart + prep * (i * spacing);
                        positions.Add(spawnPos);
                    }
                    break;
                }
                // More patterns here!
        }
        return positions;
    }
}

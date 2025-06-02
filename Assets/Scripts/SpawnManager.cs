using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnPatternType { Burst, Radial /*, Line, ZigZag */}
public class SpawnManager : MonoBehaviour {
    [Header("Enemey Types")]
    public List<GameObject> enemies;

    [Header("Spawn Settings")]
    public float spawnInterval = 3f;
    public int minEnemiesPerWave = 3;
    public int maxEnemiesPerWave = 10;

    [Header("Pattern Settings")]
    public float spawnRadius = 2f; // For burst/radial

    Camera mainCam;

    void Start() {
        mainCam = Camera.main;
        InvokeRepeating(nameof(SpawnWave), 2f, spawnInterval);
    }

    void SpawnWave() {
        // Decide what and how to spawn
        int count = Random.Range(minEnemiesPerWave, maxEnemiesPerWave);
        SpawnPatternType pattern = (SpawnPatternType)Random.Range(0, System.Enum.GetValues(typeof(SpawnPatternType)).Length);
        GameObject enemy = enemies[Random.Range(0, enemies.Count)];

        // Choose offscreen anchor
        Vector2 anchor = GetOffscreenPosition(1f);

        // Get spawn positions for the chosen pattern
        List<Vector2> spawnPosition = GeneratePattern(pattern, anchor, count);

        // Spawn!
        foreach(var pos in spawnPosition) {
            Instantiate(enemy, pos, Quaternion.identity);
        }
    }

    Vector2 GetOffscreenPosition(float buffer = 1f) {
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

    List<Vector2> GeneratePattern(SpawnPatternType pattern, Vector2 anchor, int count) {
        List<Vector2> positions = new List<Vector2>();

        switch(pattern) {
            case SpawnPatternType.Burst:
                for (int i = 0; i < count; i++) {
                    positions.Add(anchor + Random.insideUnitCircle * spawnRadius);
                }
                break;
            case SpawnPatternType.Radial:
                //TODO: Currently spawns in a small radius based on count but should spawn in a big radius outside the screen
                float angleStep = 360f / count;

                for (int i = 0; i < count; i++) {
                    float angle = angleStep * i * Mathf.Deg2Rad;
                    Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;
                    positions.Add(anchor + offset);
                }
                break;
            // More patterns here!
        }
        return positions;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnPatternType { Burst, Radial, Linear/*, ZigZag */}
public class SpawnManager : MonoBehaviour {
    [Header("Enemey Types")]
    public List<GameObject> enemies;

    [Header("Spawn Settings")]
    public float spawnInterval = 3f;
    public int minEnemiesPerWave = 3;
    public int maxEnemiesPerWave = 10;
    public float buffer = 1f;

    [Header("Pattern Settings")]
    public float spawnRadius = 2f; // For burst/radial

    Camera mainCam;
    float radius;

    void Start() {
        mainCam = Camera.main;
        // Use the camera’s orthographic size and aspect ratio to compute the edge for some spawn patterns
        radius = Mathf.Max(mainCam.orthographicSize, mainCam.orthographicSize * mainCam.aspect) + buffer;
        InvokeRepeating(nameof(SpawnWave), 2f, spawnInterval);
    }

    void SpawnWave() {
        // Decide what and how to spawn
        int count = Random.Range(minEnemiesPerWave, maxEnemiesPerWave);
        SpawnPatternType pattern = (SpawnPatternType)Random.Range(0, System.Enum.GetValues(typeof(SpawnPatternType)).Length);

        // Get spawn positions for the chosen pattern
        List<Vector2> spawnPosition = GeneratePattern(pattern, count);

        // Spawn!
        foreach(var pos in spawnPosition) {
            GameObject enemy = enemies[Random.Range(0, enemies.Count)];
            Instantiate(enemy, pos, Quaternion.identity);
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
            } case SpawnPatternType.Radial: {
                float angleStep = 360f / count;
                Vector2 camCenter = (Vector2)mainCam.transform.position;

                for (int i = 0; i < count; i++) {
                    float angle = angleStep * i * Mathf.Deg2Rad;
                    Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                    Vector2 spawnPos = camCenter + offset * radius;
                    positions.Add(spawnPos);
                }
                break;
            }  case SpawnPatternType.Linear: {
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

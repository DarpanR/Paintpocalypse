using System.Collections.Generic;
using UnityEngine;

public enum SpawnPatternType { Random, Burst, Radial, Linear /*, ZigZag */}

public class PhaseManager : MonoBehaviour {
    public static PhaseManager Instance { get; private set; }

    [Header("Enemy Phases")]
    [SerializeField] List<PhaseDefinition> phases;
    public List<PhaseDefinition> Phases { get { return phases; } }

    [Header("Pattern logic")]
    public int buffer = 1;
    public float spawnRadius = 2f;
    public float totalDuration {  get; private set; }

    int currentPhase;
    // Time tracker for current phase
    float timer;
    // Time tracker for periodic wave in each phase
    float nextWave = 0f;
    // Index tracker for rush waves
    int nextRush = 0;
    bool finished;
    bool initialized = false;

    Camera mainCam;
    float radius;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    private void Start() {
        currentPhase = 0;
        finished = false;
        initialized = true;

        mainCam = Camera.main;

        // Use the camera’s orthographic size and aspect ratio to compute the edge for some spawn patterns
        radius = Mathf.Max(mainCam.orthographicSize, mainCam.orthographicSize * mainCam.aspect) + buffer;

        // Sorts spawn waves based on their spawn timer set
        // and add phase duration to totalDuration
        foreach (var phase in phases) {
            phase.specialWaves.Sort((a, b) => a.timeOffSet.CompareTo(b.timeOffSet));
            totalDuration += phase.duration;
        }
        BeginPhase(0);
    }

    void Update() {
        if (!initialized || finished) return;
        timer += Time.deltaTime;
        var phase = phases[currentPhase];

        // Periodic Spawns
        if (timer >= nextWave) {
            FireInstruction(phase.periodicSpawns[Random.Range(0, phase.periodicSpawns.Count)]);
            nextWave += phase.periodicInterval;
        }

        // Special Spawns
        while (nextRush < phase.specialWaves.Count && timer >= phase.specialWaves[nextRush].timeOffSet) {
            foreach (var ins in phase.specialWaves[nextRush].spawns) FireInstruction(ins);
            nextRush++;
        }

        // End of phase?
        if (timer >= phase.duration) {
            int next = currentPhase + 1;

            if (next < phases.Count)
                BeginPhase(next);
            else
                finished = true;                
        }
    }

    void BeginPhase(int phaseIndex) {
        currentPhase = phaseIndex;
        timer = 0f;
        nextWave = phases[phaseIndex].periodicInterval;
        nextRush = 0;
    }

    void FireInstruction(SpawnInstruction ins) {
        int count = Random.Range(ins.minCount, ins.maxCount + 1);
        List<Vector2> positions = GeneratePattern(ins.pattern, count);

        for (int i = 0; i < positions.Count; i++) {
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
            case SpawnPatternType.Random:
                //System.Random rand = new Syste.Random();
                break;
                // More patterns here!
        }
        return positions;
    }

    public int CurrentPhase => currentPhase;
    public bool IsDone => finished;
}

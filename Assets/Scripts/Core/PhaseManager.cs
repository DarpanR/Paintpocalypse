//using System;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public enum SpawnPatternType { Random, Burst, Radial, Linear /*, ZigZag */}
public enum WaveType { Periodic, Rush , End }

public class PhaseManager : MonoBehaviour {

    public static PhaseManager Instance { get; private set; }

    public int currentPhase = 0;

    [Header("Enemy Phases")]
    [SerializeField] List<Phasedata> phases;

    [Header("Pattern logic")]
    public int buffer = 1;
    public float spawnRadius = 2f;

    public List<Phasedata> Phases { get { return phases; } }
    public float TotalElapsedTime => timeKeeper.Time;
    public float TotalDuration { get; private set; }
    public float CurrentPhaseTime => phaseTimer != null ? phaseTimer.Time : 0f;
    public float CurrentPhaseDuration => phases != null && phases.Count > 0 ? phases[CurrentPhase].duration : 0f;
    public int CurrentPhase { get; private set; }
    public bool IsDone { get; private set; }

    // Time tracker for current phase
    ClockTimer phaseTimer;
    readonly StopWatchTimer timeKeeper = new StopWatchTimer();

    List<(float, WaveType)> alarms;
    Camera mainCam;
    float radius;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
        mainCam = Camera.main;

        //temporary
        if (Phases.Count == 0) Destroy(this);

        // Sorts spawn waves based on their spawn timer set
        // and add phase duration to TotalDuration

        for (int i = currentPhase;  i < phases.Count; i++) {
            var phase = phases[i];

            phase.specialWaves.Sort((a, b) => a.timeOffSet.CompareTo(b.timeOffSet));
            TotalDuration += phase.duration;
        }
    }

    private void Start() {
        CurrentPhase = currentPhase;
        IsDone = false;

        // Use the camera’s orthographic size and aspect ratio to compute the edge for some spawn patterns
        radius = Mathf.Max(mainCam.orthographicSize, mainCam.orthographicSize * mainCam.aspect) + buffer;
        phaseTimer = new(0, new());
        phaseTimer.onAlarm += HandlePhaseAlarms;
        phaseTimer.OnTimerStop += OnPhaseTimerStop;

        BeginPhase();
        timeKeeper.Start();
    }

    void Update() {
        if (IsDone) return;
        phaseTimer.Tick(Time.deltaTime);
        timeKeeper.Tick(Time.deltaTime);
    }

    /// <summary>
    /// TODO: FIX WAVE SPAWNING BUILD CONCRETE UNDERSTANDING OF WHAT EACH TYPE OF SPAWN IS TO DO (ESPECIALLY RUSH WAVES)
    /// </summary>
    void BeginPhase() {
        var phase = phases[CurrentPhase];
        alarms = new ();

        if (phase.periodicSpawns.Count > 0) {
            int count = Mathf.CeilToInt(phase.duration / phase.periodicInterval);

            for (int i = 0; i < count; i++) {
                float t = phase.periodicInterval * i;

                alarms.Add((t, WaveType.Periodic));
            }
        }

        foreach (var waves in phase.specialWaves) 
            if (waves.spawns.Count > 0) 
                alarms.Add((waves.timeOffSet, WaveType.Rush));
        alarms.Add((CurrentPhaseDuration, WaveType.End));
        phaseTimer.Reset(alarms.Select(a => a.Item1).ToList());

    }

    void HandlePhaseAlarms(float time) {
        //Debug.Log("alarm! " + time);
        var phase = phases[CurrentPhase];
        var alarm = alarms.FindAll(a => a.Item1 == time);

        foreach (var type in alarm) {
            //Debug.Log($"alarm time: {type.Item1}, type: {type.Item2}");
            switch (type.Item2) {
                case WaveType.Periodic:
                    FireInstruction(GetWieghtedInstruction(phase.periodicSpawns));
                    break;
                case WaveType.Rush:
                    foreach (var waves in phase.specialWaves)
                        if (waves.timeOffSet == type.Item1)
                            foreach (var spawn in waves.spawns)
                                FireInstruction(spawn);
                    break;
                case WaveType.End:
                    GameEvents.RaisePhaseChange();
                    break;
                default:
                    break;
            }
        }
    }

    SpawnInstruction GetWieghtedInstruction(List<SpawnInstruction> instructions) {
        float totalWeight = 0f;

        foreach (var ins in instructions)
            totalWeight += ins.weight;
        float roll = Random.value * totalWeight;
        float cumulative = 0f;

        foreach (var ins in instructions) {
            cumulative += ins.weight;
            if (roll <= cumulative)
                return ins;
        }
        // fallback in case of round error
        return instructions[^1];
    }

    void OnPhaseTimerStop() {
        if (CurrentPhase == phases.Count - 1) {
            IsDone = true;
            GameEvents.RaiseVictory(new VictoryData {
                Type = VictoryType.StickMan,
                Message = "Stickman Wins!"
            });
            return;
        }
        CurrentPhase++;
        BeginPhase();
    }

    void FireInstruction(SpawnInstruction ins) {
        int count = Random.Range(ins.minCount, ins.maxCount + 1);
        List<Vector2> positions = GeneratePattern(ins.pattern, count);

        for (int i = 0; i < positions.Count; i++) {
            EntityManager.Instance.Spawn(ins.entityData.GUID, positions[i]);
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
                var values = Enum.GetValues(typeof(SpawnPatternType));
                var validPatterns = new List<SpawnPatternType>();
                
                foreach (SpawnPatternType p in values)
                    if (p != SpawnPatternType.Random)
                        validPatterns.Add(p);

                var randPattern = validPatterns[Random.Range(0, validPatterns.Count)];
                positions = GeneratePattern(randPattern, count);
                break;
                // More patterns here!
        }
        return positions;
    }
}

using UnityEngine;

public enum GameState { Start, Play, Pause, End }

public class GameController : MonoBehaviour {
    public System.Random RNG { get; private set; }
    public int TotalScore {  get; private set; }
    public int CurrentKillStreak { get; private set; }
    public int TotalKills {  get; private set; }

    public static GameController Instance { get; private set; }

    int lastHp;

    //Game Paramenter
    public GameState CurrentState { get; private set; }

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
        DontDestroyOnLoad(gameObject);

        CurrentState = GameState.Play;

        GameEvents.OnPausePressed += TogglePause;
        GameEvents.OnEntityDeath += UpdateScore;
        GameEvents.OnHealthBarUpdate += ResetHit;
        GameEvents.OnGameStart += GenerateSeed;
    }

    private void OnDestroy() {
        GameEvents.OnPausePressed -= TogglePause;
        GameEvents.OnEntityDeath -= UpdateScore;
        GameEvents.OnHealthBarUpdate -= ResetHit;
        GameEvents.OnGameStart -= GenerateSeed;
    }

    public void ChangeState(GameState newState) {
        CurrentState = newState;

        switch (newState) {
            case GameState.Start:
            case GameState.Play:
                Time.timeScale = 1f;
                break;
            case GameState.Pause:
            case GameState.End:
                Time.timeScale = 0f;
                break;
        }
    }

    void TogglePause() {
        if (CurrentState == GameState.Play) {
            Debug.Log("gamecontroller paused");
            GameEvents.RaisePause();
            ChangeState(GameState.Pause);
        } else if (CurrentState == GameState.Pause) {
            Debug.Log("gamecontroller play");
            GameEvents.RaiseResume();
            ChangeState(GameState.Play);
        }
    }

    void GenerateSeed() {
        var seed = Random.Range(0, int.MaxValue);
        Random.InitState(seed);
        Instance.RNG = new System.Random(seed);
    }

    void ResetHit(int currentHealth, int maxHealth) {
        if (lastHp > currentHealth) CurrentKillStreak = 0;
    }

    void UpdateScore(string guid, Vector3 position) {
        var baseEntity = EntityManager.Instance.GetEntityData(guid);

        TotalScore += baseEntity.dropEntry.Cost;
        CurrentKillStreak++;
        TotalKills++;
    }
}

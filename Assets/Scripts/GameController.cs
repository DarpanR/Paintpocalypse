using System;
using UnityEngine;

public class GameController : MonoBehaviour {
    //[Header("Player")]
    //[SerializeField] PlayerController stickMan;
    //public PlayerController mounse;

    //[Header("Controlboards")]
    //[SerializeField] PhaseManager phaseManager;

    [Header("Menu Panels")]
    public GameObject pauseMenuPanel;
    public GameObject gameOverPanel;
    public GameObject settingsPanel;
    public GameObject hudPanel;

    // Singleton instance
    public static GameController Instance { get; private set; }

    //Game Paramenter
    public enum GameState { Start, Play, Pause, End }

    public GameState CurrentState { get; private set; }

    public int Score { get; private set; } = 0;

    //public event Action OnGameOver;
    //public event Action OnWaveAdvance;
    public event Action<GameState> OnStateChanged;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    private void Start() {
        //phaseManager = phaseManager ?? GetComponent<PhaseManager>();
        ChangeState(GameState.Play);
    }

    public void ChangeState(GameState newState) {
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);

        switch (newState) {
            case GameState.Play:
                HandleResume();
                break;
            case GameState.Pause:
                HandlePause();
                break;
            case GameState.End:
                HandleGameEnd();
                break;
        }
    }

    void HandleResume() {
        Time.timeScale = 1f;

        // Close all menus.
        pauseMenuPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        settingsPanel.SetActive(false);
        hudPanel.SetActive(true);
    }

    void HandlePause() {
        Time.timeScale = 0f;

        // Load Pause Menu
        pauseMenuPanel.SetActive(true);
    }

    void HandleGameEnd() {
        Time.timeScale = 0f;

        // Load Victory/GameOver Menu
        gameOverPanel.SetActive(true);
    }

    public void AddScore(int amount) {
        Score += amount;
    }

    private void Update() {
        if (CurrentState == GameState.Play) {
            if (PhaseManager.Instance.IsDone) ChangeState(GameState.End);
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (CurrentState == GameState.Play)
                ChangeState(GameState.Pause);
            else if (CurrentState == GameState.Pause)
                ChangeState(GameState.Play);
        }
    }
}

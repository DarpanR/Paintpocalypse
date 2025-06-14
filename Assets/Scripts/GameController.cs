using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameController : MonoBehaviour {
    [Header("Player")]
    [SerializeField] PlayerController stickMan;
    //public PlayerController mounse;

    [Header("Enemy Phase Handling")]
    public PhaseManager phaseManager;
    public List<PhaseDefinition> phases;

    [Header("Menu Panels")]
    public GameObject pauseMenuPanel;
    public GameObject gameOverPanel;
    public GameObject settingsPanel;
    public HUDPanel hud;

    // Singleton instance
    public static GameController Instance { get; private set; }

    //Game Paramenter
    public enum GameState { Start, Play, Pause, End }

    public GameState CurrentState { get; private set; } = GameState.Start;

    public int Score { get; private set; } = 0;

    //public event Action OnGameOver;
    //public event Action OnWaveAdvance;
    public event Action<GameState> OnStateChanged;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        CurrentState = GameState.Start;
    }

    public void ChangeState(GameState newState) {
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);

        switch (newState) {
            case GameState.Start:
                HandlePreGame();
                break;
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
    void HandlePreGame() {
        Time.timeScale = 0f;

        // Handle Managers
        phaseManager.Init(phases);
        hud.Init(phaseManager.totalDuration, stickMan.maxHealth, stickMan.CurrentHealth);

        ChangeState(GameState.Play);
    }

    void HandleResume() {
        Time.timeScale = 1f;

        // Close all menus.
        pauseMenuPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        settingsPanel.SetActive(false);
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
        if (CurrentState != GameState.Play) return;
        if (phaseManager.IsDone) ChangeState(GameState.End);
    }
}

using System;
using System.Linq;
using UnityEngine;
using static UnityEngine.CullingGroup;

public enum GameState { Start, Play, Pause, End }


public class GameController : MonoBehaviour {
    //[Header("Player")]
    //[SerializeField] PlayerController stickMan;
    //public PlayerController mounse;

    //[Header("Controlboards")]
    //[SerializeField] PhaseManager phaseManager;
    //[SerializeField] DropManager dropManager;


    // Singleton instance
    public static GameController Instance { get; private set; }

    //Game Paramenter
    public GameState CurrentState { get; private set; }

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
        DontDestroyOnLoad(gameObject);

        CurrentState = GameState.Play;

        GameEvents.OnPausePressed += TogglePause;
    }

    private void OnDestroy() {
        GameEvents.OnPausePressed -= TogglePause;
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
}

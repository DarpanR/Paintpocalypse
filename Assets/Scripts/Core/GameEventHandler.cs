using System;

public static class GameEvents {
    public static event Action<GameState> OnGameStateChanged;

    ///Game State Changers
    public static event Action OnPause;
    public static event Action OnResume;
    public static event Action OnGameStart;
    public static event Action OnGameEnd;

    public static event Action OnPausePressed;

    public static event Action<VictoryData> OnVictory;
    public static event Action<string> OnEntityDeath;
    public static event Action<int, int> OnHealthBarUpdate;
    public static event Action<int> OnExpBarUpdate;
    public static event Action OnPhaseChange;

    public static void RaiseGameStatChanged(GameState state) => OnGameStateChanged?.Invoke(state);

    static bool GameEnd => GameController.Instance.CurrentState == GameState.End;

    public static void RaisePause() {
        GameController.Instance.ChangeState(GameState.Pause);
        OnPause?.Invoke();
    }

    public static void RaiseResume() {
        GameController.Instance.ChangeState(GameState.Play);
        OnResume?.Invoke();
    }

    public static void RaiseGameStart() {
        GameController.Instance.ChangeState(GameState.Start);
        OnGameStart?.Invoke();
    }

    public static void RaiseGameEnd() {
        GameController.Instance.ChangeState(GameState.End);
        OnGameEnd?.Invoke();
    }

    /// <summary>
    ///  change state is taken care of in gamecontroller itself
    /// </summary>
    public static void RaisePausePressed() {
        if (!GameEnd) OnPausePressed?.Invoke();
    }

    public static void RaiseEntityDeath(string guid) =>
      OnEntityDeath?.Invoke(guid);

    public static void RaiseHealthBarUpdate(int health, int maxHealth) =>
        OnHealthBarUpdate?.Invoke(health, maxHealth);

    public static void RaiseEXPBarUpdate(int exp) =>
        OnExpBarUpdate?.Invoke(exp);

    public static void RaisePhaseChange() =>
        OnPhaseChange?.Invoke();

    public static void RaiseVictory(VictoryData data) {
        RaiseGameEnd();
        OnVictory?.Invoke(data);
    }
}
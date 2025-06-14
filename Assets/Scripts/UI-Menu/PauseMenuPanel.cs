
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuPanel : GameOverPanel
{
    public void Resume() {
        GameController.Instance.ChangeState(GameController.GameState.Play);
    }
}

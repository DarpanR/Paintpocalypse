using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameOverOverlay GameOverOverlay;
    int score = 0;

    public void GameOver()      // Called when game is over (waiting for win/lose condition)
    {
        GameOverOverlay.Setup(score);
    }

    
}

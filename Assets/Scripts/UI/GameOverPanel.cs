using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameOverPanel : MonoBehaviour
{
    public TMP_Text scoreText;

    void OnEnable() {
        int score = GameController.Instance.Score;
        scoreText.text = score.ToString();
    }

    public void Restart()   // Button restart game
    {
        SceneManager.LoadScene("MainScene");
    }

    public void Quit()      // Button quit game
    {
        SceneManager.LoadScene("MainMenu");
    }
}
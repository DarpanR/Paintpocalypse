using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverOverlay : MonoBehaviour
{
    public Text scoresText;

    public void Setup(int score)    // Called to make overlay visible
    {
        gameObject.SetActive(true);
        scoresText.text = "Score: " + score.ToString();
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
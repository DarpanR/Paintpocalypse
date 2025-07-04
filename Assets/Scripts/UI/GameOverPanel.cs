using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class VictoryScreenPanel : MonoBehaviour
{
    public TMP_Text victoryText;
    public TMP_Text scoreText;

    void OnEnable() {
        GameEvents.OnVictory += ShowVictoryScreen;
    }

    private void OnDisable() {
        GameEvents.OnVictory -= ShowVictoryScreen;
    }

    void ShowVictoryScreen(VictoryData data) {
        victoryText.text = data.Message;
        scoreText.text = data.FinalScore.ToString();

        //switch (data) {
        //    case VictoryType.StickMan:
        //        victoryText.text = data.Message;
        //        break;
        //    case VictoryType.Mouse:
        //        victoryText.text = "Mouse Player Wins!";
        //        break;
        //    case VictoryType.Draw:
        //        victoryText.text = "Draw!";
        //        break;
        //}
        //victoryPanel.SetActive(true);
    }

    public void Restart()   // Button restart game
    {
        GameEvents.RaiseGameStart();
        SceneManager.LoadScene("MainScene");
    }

    public void Quit()      // Button quit game
    {
        SceneManager.LoadScene("MainMenu");
    }
}
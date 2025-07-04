using UnityEngine;

public class GUIController : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject pauseMenuPanel;
    public GameObject gameOverPanel;

    private void OnEnable() {
        GameEvents.OnPause += ShowPausePanel;
        GameEvents.OnGameEnd += ShowVictoryScreen;
        GameEvents.OnGameStart += ResetPanels;
        GameEvents.OnResume += ResetPanels;
        
    }

    private void OnDisable() {
        GameEvents.OnPause -= ShowPausePanel;
        GameEvents.OnGameEnd -= ShowVictoryScreen;
        GameEvents.OnResume -= ResetPanels;
        GameEvents.OnResume -= ResetPanels;
    }

    void ShowPausePanel() {
        pauseMenuPanel.SetActive(true);
        gameOverPanel.SetActive(false);
    }

    void ResetPanels() {
        pauseMenuPanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    void ShowVictoryScreen() {
        pauseMenuPanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }
}

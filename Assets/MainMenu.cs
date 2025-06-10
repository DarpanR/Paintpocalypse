using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGameButton()
    {
        SceneManager.LoadSceneAsync("MainScene");
    }

    public void ControlsButton()
    {
        SceneManager.LoadSceneAsync("controlMenu");
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}

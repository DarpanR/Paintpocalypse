using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGameButton()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void ControlsButton()
    {
        SceneManager.LoadSceneAsync(3);
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}

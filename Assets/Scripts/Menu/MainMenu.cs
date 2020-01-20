using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string LevelName;

    public void PlayGame()
    {
        if (LevelName != null)
            SceneManager.LoadScene(LevelName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

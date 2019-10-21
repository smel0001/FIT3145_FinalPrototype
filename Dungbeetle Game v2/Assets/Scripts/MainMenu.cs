using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void ExitGame()
    {
        Application.Quit();
    }

    public void LoadTuteLevel()
    {
        SceneManager.LoadScene("Tutorial_Level");
    }

    public void LoadEasyLevel()
    {
        SceneManager.LoadScene("Easy_Level");

    }

    public void LoadMediumLevel()
    {
        SceneManager.LoadScene("Medium_Level");

    }

    public void LoadHardLevel()
    {
        SceneManager.LoadScene("Hard_Level");

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnStartPress()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OnExitPress()
    {
        Application.Quit();
    }

    public void OnHighscorePress()
    {
        SceneManager.LoadScene("Highscore");
    }
    public void OnCreditsPress()
    {
        SceneManager.LoadScene("Credits");
    }

    public void OnBackPress()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

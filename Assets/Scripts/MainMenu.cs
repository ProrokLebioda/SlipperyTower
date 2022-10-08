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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Highscore : MonoBehaviour
{
    [SerializeField] private Button _defaultSelectedButton;
    private List<HighscoreEntry> scores = new List<HighscoreEntry>();
    public HighscoreDisplay[] highscoreDisplayArray;

    private void OnEnable()
    {
        _defaultSelectedButton.Select();
    }

    void Start()
    {
        LoadScores();
        UpdateDisplay();
    }


    /*
     *  Manage highscore
     */

    public void AddNewScore(string entryName, int entryScore)
    {
        scores.Add(new HighscoreEntry { name=entryName, score=entryScore });
    }

    public void SaveScores()
    {
        XMLManager.instance.SaveScores(scores);
    }

    public void LoadScores()
    {
        scores = XMLManager.instance.LoadScores();
    }

    private void UpdateDisplay()
    {
        scores.Sort((HighscoreEntry x, HighscoreEntry y) => y.score.CompareTo(x.score));
        for (int i = 0; i < highscoreDisplayArray.Length; i++)
        {
            if (i < scores.Count)
            {
                highscoreDisplayArray[i].DisplayHighScore(scores[i].name, scores[i].score);
            }
            else
            {
                highscoreDisplayArray[i].HideEntryDisplay();
            }
        }
        
    }


    /*
     * Manage buttons
     */
    public void OnBackToMenuPress()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

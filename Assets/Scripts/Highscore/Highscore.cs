using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Highscore : MonoBehaviour
{
    public HighscoreDisplay[] highscoreDisplayArray;
    private List<HighscoreEntry> scores = new List<HighscoreEntry>();


    void Start()
    {
        // Adds some test data
        //AddNewScore("John", 4500);
        //AddNewScore("Max", 5520);
        //AddNewScore("Dave", 380);
        //AddNewScore("Steve", 6654);
        //AddNewScore("Mike", 11021);
        //AddNewScore("Teddy", 3252);
        LoadScores();
        UpdateDisplay();
        Debug.Log(Application.persistentDataPath);
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

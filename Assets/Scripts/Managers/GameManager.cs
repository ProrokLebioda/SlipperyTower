using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    public TMP_Text scoreText;
    private int scoreValue;
    public TMP_InputField nameInput;
    public GameObject gameMenu;
    public GameObject highscoreAdd;
    public TMP_Text scoreSaveValue;
    private List<HighscoreEntry> scores = new List<HighscoreEntry>();
    private int maxHighscoreEntries = 8;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        LoadScores();
    }

    // Start is called before the first frame update
    void Start()
    {
        scoreValue = 0;
        scoreText.text = "0";

        // update highscore
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateScore(int scorePoints)
    {
        scoreValue += scorePoints;

        scoreText.text = scoreValue.ToString();
    }


    public void SaveScores()
    {
        XMLManager.instance.SaveScores(scores);
    }

    public void LoadScores()
    {
        scores = XMLManager.instance.LoadScores();
    }
    public void AddNewScore(string entryName, int entryScore)
    {
        if (scores.Count < maxHighscoreEntries)
        {
            scores.Add(new HighscoreEntry { name = entryName, score = entryScore });
        }
    }

    public void OnPlayerDeath()
    {
        gameMenu.SetActive(true);

        // if score is enough for highscore show new entry edit
        highscoreAdd.SetActive(true);
        scoreSaveValue.text = scoreValue.ToString();


    }

    public void OnSavePress()
    {
        AddNewScore(nameInput.text, scoreValue);
        SaveScores();
    }


    public void OnRestartPress()
    {
        gameMenu.SetActive(false);
        highscoreAdd.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnExitToMenuPress()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

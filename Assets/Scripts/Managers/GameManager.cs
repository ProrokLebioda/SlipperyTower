using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    public GameObject comboBar;

    public float comboDuration = 3f;
    private bool isCombo = false;
    private float lastShown;
    public float current = 0;
    public Image mask;
    private int comboMultiplier = 1;

    private Coroutine comboCoroutine;

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

    private IEnumerator UpdateComboBar()
    {
        if (comboBar.activeSelf)
        {
            GetCurrentFill();
        }

        //if (isCombo && Time.time - lastShown > comboDuration)
        //{
        //    isCombo = false;
        //}
        float timerSpeed = comboDuration / 100f;
        Debug.Log("Current: " + current + " when enter");
        Debug.Log("Timer Speed: " + timerSpeed + " when enter");
        while (current > 0)
        {
            current -= timerSpeed;
        Debug.Log("Current: " + current + " inside");
        Debug.Log("Timer Speed: " + timerSpeed + " inside");
            if (current < 0)
                current = 0;
            GetCurrentFill();
            yield return new WaitForSeconds(timerSpeed); // change to ms
        }


        Debug.Log("Combo ended");
        comboBar.SetActive(false);
        isCombo = false;
        yield return null;
        
    }

    public void UpdateScore(int platformsJumped)
    {
        //
        // Calculate combo
        //
        if (platformsJumped > 3)
        {
            if (comboCoroutine != null)
                StopCoroutine(comboCoroutine);


            current = comboDuration;
            if (!isCombo)
            {
                comboMultiplier = CalculateCombo();
                isCombo = true;
                lastShown = Time.time;
            }
            else
            {
                comboMultiplier = CalculateCombo();
                lastShown = Time.time;
            }
            comboBar.SetActive(true);
            comboCoroutine = StartCoroutine(UpdateComboBar());

        }
        




        ///

        scoreValue += platformsJumped*comboMultiplier;

        scoreText.text = scoreValue.ToString();
    }

    private int CalculateCombo()
    {
        return 1;
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
        // Work on sorted, otherwise there will be a mess
        scores.Sort((HighscoreEntry x, HighscoreEntry y) => y.score.CompareTo(x.score));
        if (scores.Count < maxHighscoreEntries)
        {
            scores.Add(new HighscoreEntry { name = entryName, score = entryScore });
            SaveScores();
        }
        else
        {
            int index = -1;
            for (int i = scores.Count - 1; i >= 0; i--)
            {
                if (scores[i].score < scoreValue)
                {
                    index = i;
                    continue;
                }
                break;
            }

            if (index > -1)
            {
                scores.Insert(index, new HighscoreEntry { name = entryName, score = scoreValue });
                scores.RemoveAt(scores.Count-1);
                SaveScores();
            }
        }
    }

    public void OnPlayerDeath()
    {
        gameMenu.SetActive(true);
        gameMenu.transform.Find("RestartButton").GetComponent<Button>().Select();
        // if score is enough for highscore show new entry edit
        if (scores.Count < maxHighscoreEntries || (scores.Count > 1 && scores[scores.Count-1].score < scoreValue))
        {
            highscoreAdd.SetActive(true);
            scoreSaveValue.text = scoreValue.ToString();
        }

    }

    public void OnSavePress()
    {
        // Validate
        AddNewScore(nameInput.text, scoreValue);
        
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


    public void GetCurrentFill()
    {
        float fillAmount = current / comboDuration;
        mask.fillAmount = fillAmount;
    }

}

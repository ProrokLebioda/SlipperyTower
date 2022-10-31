using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    
    // UI
    public GameObject gameMenu;
    public GameObject comboBar;
    public GameObject highscoreAdd;

    // Score
    public TMP_Text scoreText;
    private int scoreValue;
    public TMP_InputField nameInput;
    public TMP_Text scoreSaveValue;
    private List<HighscoreEntry> scores = new List<HighscoreEntry>();
    private int maxHighscoreEntries = 8;


    // Combo
    public float comboDuration = 3f;
    private bool isCombo = false;
    public float currentFill = 0;
    private int baseComboMultiplier = 1;
    private int currentComboMultiplier = 1;
    public Image comboBarMask;
    private int levelsInCombo = 0;
    private int levelsForComboIncrease = 30;

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
            comboBar.transform.Find("ComboValue").GetComponent<TMP_Text>().SetText(currentComboMultiplier.ToString());
        }
                
        float timerSpeed = comboDuration / 100f;
        //Debug.Log("Current: " + currentFill + " when enter");
        //Debug.Log("Timer Speed: " + timerSpeed + " when enter");
        while (currentFill > 0)
        {
            currentFill -= timerSpeed;
        //Debug.Log("Current: " + currentFill + " inside");
        //Debug.Log("Timer Speed: " + timerSpeed + " inside");
            if (currentFill < 0)
                currentFill = 0;
            comboBar.transform.Find("ComboValue").GetComponent<TMP_Text>().SetText(currentComboMultiplier.ToString());
            GetCurrentFill();
            yield return new WaitForSeconds(timerSpeed); // change to ms
        }


        Debug.Log("Combo ended");
        comboBar.SetActive(false);
        isCombo = false;
        currentComboMultiplier = baseComboMultiplier;
        levelsInCombo = 0;
        yield return null;
        
    }

    public void UpdateScore(int platformsJumped)
    {
        if (isCombo)
        {
            levelsInCombo += platformsJumped;
        }
        if (platformsJumped > 3)
        {
            if (comboCoroutine != null)
            {
                StopCoroutine(comboCoroutine);
            }

            currentFill = comboDuration;
            if (!isCombo)
            {
                currentComboMultiplier = CalculateCombo(platformsJumped);
                isCombo = true;
            }
            else
            {
                currentComboMultiplier = CalculateCombo(platformsJumped);
            }
            comboBar.SetActive(true);
            comboCoroutine = StartCoroutine(UpdateComboBar());
        }
        

        scoreValue += platformsJumped*currentComboMultiplier;

        scoreText.text = scoreValue.ToString();
    }

    private int CalculateCombo(int platformsJumped)
    {
        int levelsNeeded = levelsForComboIncrease * currentComboMultiplier;
        if (levelsInCombo > 0 && levelsInCombo - levelsNeeded > 0)
        {
            levelsInCombo -= levelsNeeded;
            currentComboMultiplier*=2;

        }
        if (currentComboMultiplier == 1)
            currentComboMultiplier++;

        return currentComboMultiplier;

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
                scores.RemoveAt(scores.Count - 1);
                SaveScores();
            }
        }
    }

    public void OnPlayerDeath()
    {
        if (comboCoroutine != null)
        {
            isCombo = false;
            baseComboMultiplier = 1;
            levelsInCombo = 0;
            StopCoroutine(comboCoroutine);
        }
        comboBar.SetActive(false);

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
        float fillAmount = currentFill / comboDuration;
        comboBarMask.fillAmount = fillAmount;
    }

}

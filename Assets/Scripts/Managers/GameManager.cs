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
    [SerializeField] private GameObject gameMenu;
    [SerializeField] private GameObject comboBar;
    [SerializeField] private GameObject highscoreAdd;

    // Pause menu
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _resumeButton;

    // Score
    private int scoreValue;
    private List<HighscoreEntry> scores = new List<HighscoreEntry>();
    private int maxHighscoreEntries = 8;
    private int playerFloor;
    public TMP_Text scoreText;
    public TMP_InputField nameInput;
    public TMP_Text scoreSaveValue;
   

    // Combo
    private bool isCombo = false;
    private int baseComboMultiplier = 1;
    private int currentComboMultiplier = 1;
    private int levelsInCombo = 0;
    private int levelsForComboIncrease = 30;
    public float comboDuration = 3f;
    public float currentFill = 0;
    public Image comboBarMask;

    private Coroutine comboCoroutine;

    private int highestFloorReached;
    public int HighestFloorReached { get => highestFloorReached; set => highestFloorReached = value; }
    public int PlayerFloor { get => playerFloor; set => playerFloor = value; }

    public GameObject particleObject;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        Time.timeScale = 1.0f;

        particleObject.SetActive(false);
        LoadScores();
    }

    // Start is called before the first frame update
    void Start()
    {
        scoreValue = 0;
        scoreText.text = "0";
        PlayerFloor = -1;
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
        while (currentFill > 0)
        {
            currentFill -= timerSpeed;
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

    public void AddGoldCoinValue(int goldCoinValue)
    {
        scoreValue += goldCoinValue;
        scoreText.text = scoreValue.ToString();
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
            //spawn particles
            particleObject.transform.position = GameObject.Find("Player").transform.position;
            particleObject.SetActive(false);
            particleObject.SetActive(true);
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
        scores.Sort((HighscoreEntry x, HighscoreEntry y) => y.score.CompareTo(x.score));
        XMLManager.instance.SaveScores(scores);
    }

    public void LoadScores()
    {
        scores = XMLManager.instance.LoadScores();
        // Work on sorted, otherwise there will be a mess
        scores.Sort((HighscoreEntry x, HighscoreEntry y) => y.score.CompareTo(x.score));
        if (scores.Count > 0)
        {
            int maxFloor = -1;
            foreach(HighscoreEntry score in scores)
            {
                maxFloor = score.floor > maxFloor ? score.floor : maxFloor;
            }

            highestFloorReached = maxFloor;
        }
    }
    public void AddNewScore(string entryName, int entryScore, int entryFloor)
    {
        // Work on sorted, otherwise there will be a mess
        if (scores.Count < maxHighscoreEntries)
        {
            scores.Add(new HighscoreEntry { name = entryName, score = entryScore, floor = entryFloor});
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
                scores.Insert(index, new HighscoreEntry { name = entryName, score = scoreValue, floor = entryFloor });
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

        FindObjectOfType<AudioManager>().Play("PlayerDeath");

        gameMenu.SetActive(true);
        gameMenu.transform.Find("RestartButton").GetComponent<Button>().Select();

        // if score is enough for highscore show new entry edit
        if (scores.Count < maxHighscoreEntries || (scores.Count > 1 && scores[scores.Count-1].score < scoreValue))
        {
            highscoreAdd.SetActive(true);
            scoreSaveValue.text = scoreValue.ToString();
            gameMenu.transform.Find("HighscoreAdd/NameEdit").GetComponent<TMP_InputField>().Select();
        }
    }

    public void OnSavePress()
    {
        // Validate
        AddNewScore(nameInput.text, scoreValue, PlayerFloor);
        highscoreAdd.SetActive(false);
        gameMenu.transform.Find("RestartButton").GetComponent<Button>().Select();
    }

    public void OnRestartPress()
    {
        //Time.timeScale = 1.0f;
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

    public void PauseMenu()
    {
        //OnPlayerDeath();
        Time.timeScale = 0.0f;
        _pauseMenu.SetActive(true);
        _pauseMenu.transform.Find("ResumeGameButton").GetComponent<Button>().Select();

    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
        _pauseMenu.SetActive(false);
    }    
}

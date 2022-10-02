using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    public TMP_Text scoreText;
    private int scoreValue;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        scoreValue = 0;
        scoreText.text = "0";
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

}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighscoreDisplay : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text scoreText;

    public void DisplayHighScore(string name, int score)
    {
        nameText.text = name;
        scoreText.text = score.ToString();
    }

    public void HideEntryDisplay()
    {
        nameText.text = "";
        scoreText.text = "";
    }

}

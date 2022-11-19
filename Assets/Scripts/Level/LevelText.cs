using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelText : MonoBehaviour
{
    public TMP_Text m_levelText;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void PlaceAtPosition(Vector2 position)
    {
        transform.position = position;
    }

    public void SetText(string text)
    {
        m_levelText.text = text;
    }
}

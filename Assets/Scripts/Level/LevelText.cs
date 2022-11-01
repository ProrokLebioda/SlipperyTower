using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelText : MonoBehaviour
{

    public TMP_Text levelText;

    private void OnEnable()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public void PlaceAtPosition(Vector2 position)
    {
        transform.position = position;
    }

    public void SetText(string text)
    {
        levelText.text = text;
    }
}

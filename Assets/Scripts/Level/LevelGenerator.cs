using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{
    
    public Tilemap platformTilemap;
    public RuleTile ruleTile;
    void Start()
    {
        GeneratePlatforms();
    }

    private void GeneratePlatforms()
    {
        platformTilemap.SetTile(new Vector3Int(0, 0, 0), ruleTile);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

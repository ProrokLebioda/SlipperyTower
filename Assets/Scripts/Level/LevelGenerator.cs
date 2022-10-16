using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{

    private int leftMostX = -5;
    private int rightMostX = 4;
    private int bottomY = 0;

    public int sectionHeight = 20;

    public int testLevels = 200;

    public Tilemap platformTilemap;
    public Tilemap wallsTilemap;
    public Tilemap collisionWallsTilemap;
    public RuleTile platformRuleTile;
    public RuleTile wallRuleTile;
    void Start()
    {
        GeneratePlatforms();


    }

    private void GeneratePlatforms()
    {
        int horizontalSpacing = 1; // 1 means every tile
        // test generate for 20 levels
        // from leftMostX and bottomY to rightMostX and sectionHeight
        //for (int y = bottomY; y < sectionHeight; y++)
        for (int y = bottomY; y < testLevels; y++)
        {
            for (int x = leftMostX - 4; x < leftMostX; x++)
            {
                wallsTilemap.SetTile(new Vector3Int(x, y, 0), wallRuleTile);
            }

            // Section division deduce 1 to make it work
            if (y > 0 && y % (sectionHeight) == 0)
            {
                horizontalSpacing = 1;
                for (int x = leftMostX; x <= rightMostX; x++)
                {
                    platformTilemap.SetTile(new Vector3Int(x, y, 0), platformRuleTile);
                }
            }
            else
            {
                // Add some code to handle drawing tiles for normal platforms
                //
                PlaceRandomPlatforms(y);
            }

            for (int x = rightMostX + 1 ; x <= rightMostX+4; x++)
            {
                wallsTilemap.SetTile(new Vector3Int(x, y, 0), wallRuleTile);
            }
        }
    }

    private void PlaceRandomPlatforms(int y)
    {
        if (y % 2 == 0)
            return;
        int horizontalSpacing = 3;

        // for the ease of creating do a 5 tile platforms only, 1 per level

        bool isPlaceable = false;

        int leftEdgeOfPlatform = -100;
        int platformSize = 5;

        while (!isPlaceable)
        {
            leftEdgeOfPlatform = UnityEngine.Random.Range(leftMostX, rightMostX);

            if (leftEdgeOfPlatform+platformSize <= rightMostX)
            {
                isPlaceable = true;
            }
        }

        for (int x = leftEdgeOfPlatform; x <= leftEdgeOfPlatform+rightMostX; x += 1)
        {
            platformTilemap.SetTile(new Vector3Int(x, y, 0), platformRuleTile);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ShouldGenerate())
        {



        }
    }

    bool ShouldGenerate()
    {
        

        return true;
    }
}

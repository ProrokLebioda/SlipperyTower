using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{

    private int leftMostX = -5;
    private int rightMostX = 4;
    private int bottomY = 1;

    public int sectionHeight = 20;
    private int currentPlayerSection = 0;
    private int platformSize = 5;

    public int startSectionLevels = 20;

    public Tilemap platformTilemap;
    public Tilemap wallsTilemap;
    public Tilemap collisionWallsTilemap;
    public RuleTile platformRuleTile;
    public RuleTile wallRuleTile;


    public GameObject levelIndicator;

    public GameObject playerCamera;

    private GameObject player;
    void Start()
    {
        GeneratePlatforms();
        player = GameObject.FindWithTag("Player");

    }

    private void FixedUpdate()
    {
        // Check player position

        if (player != null)
        {
            int playerY = Mathf.RoundToInt(playerCamera.transform.position.y);
            if (playerY > (currentPlayerSection * sectionHeight + (sectionHeight / 2)))
            {
                
                //workswonky for some reason...
                GeneratePlatforms((sectionHeight + 1) + sectionHeight * (currentPlayerSection), (sectionHeight) + sectionHeight * (currentPlayerSection + 1));
                currentPlayerSection++;
                if (platformSize >= 4 && currentPlayerSection % 5 == 0)
                    platformSize--;

                if (currentPlayerSection % 3 == 0)
                {
                    playerCamera.GetComponent<FollowCamera>().cameraSpeed += 0.2f;
                }
            }

            //if (playerY > currentPlayerSection * sectionHeight + sectionHeight)
            //{
               
            //}
        }
    }


    private void GeneratePlatforms()
    {
        GeneratePlatforms(bottomY, sectionHeight);
        //// test generate for 20 levels
        //// from leftMostX and bottomY to rightMostX and sectionHeight
        ////for (int y = bottomY; y < sectionHeight; y++)
        //for (int y = bottomY; y <= sectionHeight; y++)
        //{
        //    for (int x = leftMostX - 4; x < leftMostX; x++)
        //    {
        //        wallsTilemap.SetTile(new Vector3Int(x, y, 0), wallRuleTile);

        //    }

        //    //Section division
        //    if (y > 0 && y % (sectionHeight) == 0)
        //    {
        //        for (int x = leftMostX; x <= rightMostX; x++)
        //        {
        //            platformTilemap.SetTile(new Vector3Int(x, y, 0), platformRuleTile);
        //            if (x == (leftMostX + rightMostX) / 2)
        //            {
        //                GameObject go = Instantiate(levelIndicator, new Vector3(x, y, 0), Quaternion.identity);
        //                LevelText lt = go.GetComponent<LevelText>();
        //                lt.SetText(y.ToString());
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // Add some code to handle drawing tiles for normal platforms
        //        //
        //        PlaceRandomPlatforms(y);
        //    }

        //    for (int x = rightMostX + 1 ; x <= rightMostX+4; x++)
        //    {
        //        wallsTilemap.SetTile(new Vector3Int(x, y, 0), wallRuleTile);
        //    }
        //}
    }

    private void GeneratePlatforms(int fromY,int toY)
    {
        // test generate for 20 levels
        // from leftMostX and bottomY to rightMostX and sectionHeight
        //for (int y = bottomY; y < sectionHeight; y++)
        for (int y = fromY; y <= toY; y++)
        {
            for (int x = leftMostX - 4; x < leftMostX; x++)
            {
                wallsTilemap.SetTile(new Vector3Int(x, y, 0), wallRuleTile);
                
            }

            // Section division 
            if (y > 0 && y % (sectionHeight) == 0)
            {
                for (int x = leftMostX; x <= rightMostX; x++)
                {
                    platformTilemap.SetTile(new Vector3Int(x, y, 0), platformRuleTile);

                    if (x == (leftMostX + rightMostX) / 2)
                    {
                        GameObject go = Instantiate(levelIndicator, new Vector3(x, y, 0), Quaternion.identity);
                        LevelText lt = go.GetComponent<LevelText>();
                        lt.SetText(y.ToString());
                    }
                }
            }
            else
            {
                // Add some code to handle drawing tiles for normal platforms
                //
                PlaceRandomPlatforms(y);
            }

            for (int x = rightMostX + 1; x <= rightMostX + 4; x++)
            {
                wallsTilemap.SetTile(new Vector3Int(x, y, 0), wallRuleTile);
            }
        }
    }

    private void PlaceRandomPlatforms(int y)
    {
        if (y % 2 == 1)
            return;

        // for the ease of creating do a 5 tile platforms only, 1 per level

        bool isPlaceable = false;

        int leftEdgeOfPlatform = -100;
        

        while (!isPlaceable)
        {
            leftEdgeOfPlatform = UnityEngine.Random.Range(leftMostX, rightMostX);

            if (leftEdgeOfPlatform+platformSize <= rightMostX)
            {
                isPlaceable = true;
            }
        }

        for (int x = leftEdgeOfPlatform; x <= leftEdgeOfPlatform + platformSize; x += 1)
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

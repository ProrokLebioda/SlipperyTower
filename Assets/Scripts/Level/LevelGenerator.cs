using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{
    private readonly int leftMostX = -5;
    private readonly int rightMostX = 4;
    private readonly int bottomY = 1;

    private int currentPlayerSection = 0;
    private int platformSize = 5;
    public int sectionHeight = 20;
    public int startSectionLevels = 20;
    private int highestFloorReached;

    private GameObject player;
    private RuleTile currentRuleBrush;
    public RuleTile[] platformRuleTiles;
    public RuleTile wallRuleTile;
    public Tilemap platformTilemap;
    public Tilemap wallsTilemap;
    public Tilemap collisionWallsTilemap;
    public GameObject levelIndicator;
    public GameObject highestLevelReachedAsset;
    public GameObject playerCamera;

    public GameObject Background;

    void Start()
    {
        currentRuleBrush = platformRuleTiles[0];
        player = GameObject.FindWithTag("Player");
        highestFloorReached = GameManager.Instance.HighestFloorReached;
        GeneratePlatforms();
    }

    private void FixedUpdate()
    {
        UpdatePlatformsBasedOnPlayerPosition();
    }

    private void UpdatePlatformsBasedOnPlayerPosition()
    {
        if (player != null)
        {
            int playerY = Mathf.RoundToInt(playerCamera.transform.position.y);
            if (playerY > (currentPlayerSection * sectionHeight + (sectionHeight / 2)))
            {
                //workswonky for some reason...
                GeneratePlatforms((sectionHeight + 1) + sectionHeight * (currentPlayerSection), (sectionHeight) + sectionHeight * (currentPlayerSection + 1));
                currentPlayerSection++;
                if (currentPlayerSection >= 1 && currentPlayerSection < platformRuleTiles.Length)
                {
                    currentRuleBrush = platformRuleTiles[currentPlayerSection];
                }

                if (platformSize >= 4 && currentPlayerSection % 5 == 0)
                    platformSize--;

                if (currentPlayerSection % 3 == 0)
                {
                    playerCamera.GetComponent<FollowCamera>().cameraSpeed += 0.2f;
                }
            }

        }
    }

    private void GeneratePlatforms()
    {
        GeneratePlatforms(bottomY, sectionHeight);
    }

    private void GeneratePlatforms(int fromY,int toY)
    {
        for (int y = fromY; y <= toY; y++)
        {
            PlaceWalls(leftMostX - 4, leftMostX, y);

            // Section division 
            if (y > 0 && y % (sectionHeight) == 0)
            {
                PlaceSectionDivision(y);
            }
            else
            {
                PlaceRandomPlatforms(y);
            }

            PlaceWalls(rightMostX + 1, rightMostX + 4, y);            
        }
        PlaceBackground(fromY);
    }

    private void PlaceBackground(int fromY)
    {
        GameObject go = BackgroundPool.Instance.GetPooledGameObject();
        if (go != null)
        {
            Background bg = go.GetComponent<Background>();
            bg.PlaceAtPosition(new Vector2(0, fromY));
            go.SetActive(true);
        }
    }



    private void PlaceWalls(int from, int to, int y)
    {
        for (int x = from; x < to; x++)
        {
            wallsTilemap.SetTile(new Vector3Int(x, y, 0), wallRuleTile);
        }
    }

    private void PlaceSectionDivision(int y)
    {
        for (int x = leftMostX; x <= rightMostX; x++)
        {
            platformTilemap.SetTile(new Vector3Int(x, y, 0), currentRuleBrush);

            if (x == (leftMostX + rightMostX) / 2 && y > 0)
            {
                GameObject go = Instantiate(levelIndicator, new Vector3(x, y, 0), Quaternion.identity);
                LevelText lt = go.GetComponent<LevelText>();
                lt.SetText(y.ToString());
            }
        }
    }

    private void PlaceRandomPlatforms(int y)
    {
        if (y % 2 == 1)
            return;

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
            if (y == highestFloorReached && x == leftEdgeOfPlatform + 1 ) 
            {
                // for now place flag for highest reached floor on second from left
                GameObject go = Instantiate(highestLevelReachedAsset, new Vector3(x, y+1, 0), Quaternion.identity); // +1 for y, because of difference between grid and coord system
                LevelText lt = go.GetComponent<LevelText>();
                lt.SetText(y.ToString());
            }
            platformTilemap.SetTile(new Vector3Int(x, y, 0), currentRuleBrush);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}

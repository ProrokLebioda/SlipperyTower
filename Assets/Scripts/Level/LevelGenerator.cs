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

    [SerializeField]
    private int minimumPlatformWidth = 3;
    private int currentPlayerSection = 0;
    private int platformSize = 5;
    public int sectionHeight = 20;
    [SerializeField]
    private int loadSectionHeight = 20;
    private int currentLoadedSections = 0;
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

    [SerializeField]
    private GameObject CoinObject;

    private int oldPlayerY;

    void Start()
    {
        currentRuleBrush = platformRuleTiles[0];
        player = GameObject.FindWithTag("Player");
        oldPlayerY = Mathf.RoundToInt(playerCamera.transform.position.y);
        highestFloorReached = GameManager.Instance.HighestFloorReached;
        currentLoadedSections = 0;
        GeneratePlatforms();
    }

    private void FixedUpdate()
    {
        //UpdatePlatformsBasedOnPlayerPosition();
    }

    private void UpdatePlatformsBasedOnPlayerPosition()
    {
        if (player != null)
        {
            int playerY = Mathf.RoundToInt(playerCamera.transform.position.y);
            if (oldPlayerY < playerY && playerY > currentLoadedSections)
            {
                if (IsPlatformsLoadNeeded(playerY))
                {
                    //workswonky for some reason...
                    int fromLevel = (loadSectionHeight + 1) + loadSectionHeight * (currentPlayerSection);
                    int toLevel = (loadSectionHeight) + loadSectionHeight * (currentPlayerSection + 1);
                    GeneratePlatforms(fromLevel , toLevel );
                    currentLoadedSections = toLevel;
                    if (IsPlayerSectionIncreaseNeeded(playerY))
                    {
                        currentPlayerSection++;
                    }

                    if (IsRuleBrushSwitchNeeded())
                    {
                        currentRuleBrush = platformRuleTiles[currentPlayerSection];
                    }

                    if (IsPlatfromSizeShrinkNeeded())
                        platformSize--;

                    if (IsCameraSpeedIncreaseNeeded())
                    {
                        playerCamera.GetComponent<FollowCamera>().cameraSpeed += 0.2f;
                    }
                }
                oldPlayerY = playerY;

                CleanupGrid();
            }

        }
    }

    private void CleanupGrid()
    {
        // clean everything from below current section

        for (int i = (currentPlayerSection-1) * sectionHeight; i > 0; i--)
        {
            for (int j = leftMostX - 6; j < leftMostX; j++)
            {
                wallsTilemap.SetTile(new Vector3Int(j, i, 0), null);
            }
            for (int j = leftMostX; j < rightMostX +1; j++)
            {
                platformTilemap.SetTile(new Vector3Int(j, i, 0), null);
            }
            for (int j = rightMostX + 1; j < rightMostX + 7; j++)
            {
                wallsTilemap.SetTile(new Vector3Int(j, i, 0), null);
            }
        }
    }

    private bool IsCameraSpeedIncreaseNeeded()
    {
        return currentPlayerSection % 3 == 0;
    }

    private bool IsPlatfromSizeShrinkNeeded()
    {
        return platformSize >= minimumPlatformWidth && currentPlayerSection % 5 == 0;
    }

    private bool IsPlayerSectionIncreaseNeeded(int playerY)
    {
        return playerY > (currentPlayerSection * sectionHeight);
    }

    private bool IsRuleBrushSwitchNeeded()
    {
        return currentPlayerSection >= 1 && currentPlayerSection < platformRuleTiles.Length;
    }

    private bool IsPlatformsLoadNeeded(int playerY)
    {
        return playerY > (currentPlayerSection * sectionHeight - (loadSectionHeight / 2));
    }

    private void GeneratePlatforms()
    {
        GeneratePlatforms(bottomY, loadSectionHeight);
    }

    private void GeneratePlatforms(int fromY,int toY)
    {
        for (int y = fromY; y <= toY; y++)
        {
            PlaceWalls(leftMostX - 6, leftMostX, y);

            // Section division 
            if (y > 0 && y % (sectionHeight) == 0)
            {
                PlaceSectionDivision(y);
            }
            else
            {
                PlaceRandomPlatforms(y);
            }
            PlaceCoins(y);
            PlaceWalls(rightMostX + 1, rightMostX + 7, y);            
        }
        PlaceBackground(fromY);        
    }

    private void PlaceCoins(int y)
    {
        if ( y % 2 == 0)
            return;

        //Random amount between min and max
        int[] coinMask = new int[10];
        for (int a = 0; a<10;a++)
        {
            int coinSpot = UnityEngine.Random.Range(0, 100);
            coinMask[a] = coinSpot > 75? 1:0;
        }

        int spot = 0;
        for (int x = leftMostX; x <= rightMostX; x ++)
        {
            if (coinMask[spot] != 0)
            {
                GameObject go = CoinsPool.Instance.GetPooledGameObject();
                if (go != null)
                {
                    go.transform.position = new Vector2(x + .5f, y + .5f);
                    go.SetActive(true);
                }
            }
            spot++;
        }
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
            PlaceLevelIndicator(y, x);
        }
    }

    private void PlaceLevelIndicator(int y, int x)
    {
        if (x == (leftMostX + rightMostX) / 2 && y > 0)
        {
            GameObject go = Instantiate(levelIndicator, new Vector3(x, y, 0), Quaternion.identity);
            LevelText lt = go.GetComponent<LevelText>();
            lt.SetText(y.ToString());
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
            PlaceHighestLevelReachedSign(y, leftEdgeOfPlatform, x);
            platformTilemap.SetTile(new Vector3Int(x, y, 0), currentRuleBrush);
        }
    }

    private void PlaceHighestLevelReachedSign(int y, int leftEdgeOfPlatform, int x)
    {
        if (y == highestFloorReached && x == leftEdgeOfPlatform + 1)
        {
            // for now place flag for highest reached floor on second from left
            GameObject go = Instantiate(highestLevelReachedAsset, new Vector3(x, y + 1, 0), Quaternion.identity); // +1 for y, because of difference between grid and coord system
            LevelText lt = go.GetComponent<LevelText>();
            lt.SetText(y.ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlatformsBasedOnPlayerPosition();
    }
}

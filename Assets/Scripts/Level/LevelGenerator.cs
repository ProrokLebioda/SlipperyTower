using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{
    private readonly int _leftMostX = -5;
    private readonly int _rightMostX = 4;
    private readonly int _bottomY = 1;

    [SerializeField]
    private int _minimumPlatformWidth = 3;
    private int _currentPlayerSection = 0;
    private int _platformSize = 5;

    // Keep sectionHeight and tilesetSwitchDivider as values possible to divide (section/tilesetSwitchDivider)
    [SerializeField]
    private int _sectionHeight = 20;
    [SerializeField]
    private int _loadSectionHeight = 20;
    private int _currentLoadedSectionsMaxY = 0;
    public int _startSectionLevels = 20;
    private int _highestFloorReached;
    private int _oldPlayerY;

    [SerializeField]
    private int _platformShrinkMagnitude = 2;
    [SerializeField]
    private int _cameraSpeedupMagnitude = 3;
    [SerializeField]
    private float _cameraSpeedUp = 0.2f;
    [SerializeField]
    private int _tilesetSwitchDivider = 100; // After how many levels tiles change


    private GameObject _player;
    private RuleTile _currentRuleBrush;
    public RuleTile[] PlatformRuleTiles;
    public RuleTile WallRuleTile;
    public Tilemap PlatformTilemap;
    public Tilemap WallsTilemap;
    public Tilemap CollisionWallsTilemap;
    public GameObject LevelIndicator;
    public GameObject HighestLevelReachedAsset;
    public GameObject PlayerCamera;

    public GameObject Background;

    [SerializeField]
    private GameObject CoinObject;

    void Start()
    {
        _currentRuleBrush = PlatformRuleTiles[0];
        _currentPlayerSection = 0;
        _player = GameObject.FindWithTag("Player");
        _oldPlayerY = Mathf.RoundToInt(PlayerCamera.transform.position.y);
        _highestFloorReached = GameManager.Instance.HighestFloorReached;
        _currentLoadedSectionsMaxY = 0;
        GeneratePlatforms();
    }

    private void FixedUpdate()
    {
        //UpdatePlatformsBasedOnPlayerPosition();
    }

    private void UpdatePlatformsBasedOnPlayerPosition()
    {
        if (_player != null)
        {
            int playerCameraY = Mathf.RoundToInt(PlayerCamera.transform.position.y);

            if (playerCameraY > _oldPlayerY && playerCameraY > (_currentLoadedSectionsMaxY - _loadSectionHeight))
            {
                if (IsPlatformsLoadNeeded(playerCameraY))
                {
                    //workswonky for some reason...
                    int fromLevel = _currentLoadedSectionsMaxY + 1;
                    int toLevel = fromLevel + _loadSectionHeight;
                    
                    if (IsPlayerSectionIncreaseNeeded(playerCameraY))
                    {
                        _currentPlayerSection++;                        

                    }
                    
                    if (IsCameraSpeedIncreaseNeeded())
                    {
                        PlayerCamera.GetComponent<FollowCamera>().cameraSpeed += _cameraSpeedUp;
                        Debug.Log("Camera Speed: " + PlayerCamera.GetComponent<FollowCamera>().cameraSpeed);
                    }

                    GeneratePlatforms(fromLevel , toLevel );
                }
                _oldPlayerY = playerCameraY;

                CleanupGrid();
            }

        }
    }

    private void CleanupGrid()
    {
        // clean everything from below current section

        for (int i = (_currentPlayerSection-1) * _sectionHeight; i > 0; i--)
        {
            for (int j = _leftMostX - 6; j < _leftMostX; j++)
            {
                WallsTilemap.SetTile(new Vector3Int(j, i, 0), null);
            }
            for (int j = _leftMostX; j < _rightMostX +1; j++)
            {
                PlatformTilemap.SetTile(new Vector3Int(j, i, 0), null);
            }
            for (int j = _rightMostX + 1; j < _rightMostX + 7; j++)
            {
                WallsTilemap.SetTile(new Vector3Int(j, i, 0), null);
            }
        }
    }

    private bool IsCameraSpeedIncreaseNeeded()
    {
        return _currentPlayerSection % _cameraSpeedupMagnitude == 0 && (_currentPlayerSection * _sectionHeight) % _tilesetSwitchDivider == 0;
    }

    private bool IsPlatfromSizeShrinkNeeded(int playerY)
    {
        //return _currentPlayerSection >= (_sectionHeight / _tilesetSwitchDivider) && playerY % (_sectionHeight * _currentPlayerSection) == 0 && _platformSize >= _minimumPlatformWidth && _currentPlayerSection % _platformShrinkMagnitude == 0 /*&& (_currentPlayerSection * _sectionHeight) % _tilesetSwitchDivider == 0*/;
        return _platformSize >= _minimumPlatformWidth && _currentPlayerSection >= (_sectionHeight / _tilesetSwitchDivider) && _currentPlayerSection % _platformShrinkMagnitude == 0 /*&& (_currentPlayerSection * _sectionHeight) % _tilesetSwitchDivider == 0*/;
    }

    private bool IsPlayerSectionIncreaseNeeded(int playerY)
    {
        return playerY > ((_currentPlayerSection + 1) * _sectionHeight);
    }

    private bool IsRuleBrushSwitchNeeded(int y)
    {
        return _currentPlayerSection >= (_tilesetSwitchDivider/_sectionHeight) && Mathf.CeilToInt(_currentPlayerSection / (_tilesetSwitchDivider / _sectionHeight)) < PlatformRuleTiles.Length && y % _tilesetSwitchDivider == 0;
    }

    /// <summary>
    /// Load happens when _player Y position is larger than halfway through _loadSectionHeight for latest loadSection
    /// </summary>
    /// <param name="playerY"></param>
    /// <returns></returns>
    private bool IsPlatformsLoadNeeded(int playerY)
    {
        return playerY > (_currentLoadedSectionsMaxY - _loadSectionHeight);
    }

    private void GeneratePlatforms()
    {
        GeneratePlatforms(_bottomY, _loadSectionHeight);
    }

    private void GeneratePlatforms(int fromY,int toY)
    {
        PlaceBackground(fromY);        
        for (int y = fromY; y <= toY; y++)
        {
            PlaceWalls(_leftMostX - 6, _leftMostX, y);

            if (IsRuleBrushSwitchNeeded(y))
            {
                int divider = _tilesetSwitchDivider / _sectionHeight;
                int brushNr = Mathf.FloorToInt(_currentPlayerSection / divider);
                _currentRuleBrush = PlatformRuleTiles[brushNr];
            }
            
            if (y > 0 && y % (_sectionHeight) == 0)
            {
                if (IsPlatfromSizeShrinkNeeded(y))
                {
                    _platformSize--;
                    Debug.Log("Platform Size: " + _platformSize);
                }
            }

            // Section division 

            if (y > 0 && y % (_sectionHeight) == 0)
            {
                PlaceSectionDivision(y);
                
            }
            else
            {
                PlaceRandomPlatforms(y);
            }
            PlaceCoins(y);
            PlaceWalls(_rightMostX + 1, _rightMostX + 7, y);            
        }
        _currentLoadedSectionsMaxY = toY;
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
        for (int x = _leftMostX; x <= _rightMostX; x ++)
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
            WallsTilemap.SetTile(new Vector3Int(x, y, 0), WallRuleTile);
        }
    }

    private void PlaceSectionDivision(int y)
    {
        for (int x = _leftMostX; x <= _rightMostX; x++)
        {
            PlatformTilemap.SetTile(new Vector3Int(x, y, 0), _currentRuleBrush);
            PlaceLevelIndicator(y, x);
        }
    }

    private void PlaceLevelIndicator(int y, int x)
    {
        if (x == (_leftMostX + _rightMostX) / 2 && y > 0)
        {
            GameObject go = Instantiate(LevelIndicator, new Vector3(x, y, 0), Quaternion.identity);
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
            leftEdgeOfPlatform = UnityEngine.Random.Range(_leftMostX, _rightMostX);

            if (leftEdgeOfPlatform+_platformSize <= _rightMostX)
            {
                isPlaceable = true;
            }
        }

        for (int x = leftEdgeOfPlatform; x <= leftEdgeOfPlatform + _platformSize; x += 1)
        {
            PlaceHighestLevelReachedSign(y, leftEdgeOfPlatform, x);
            PlatformTilemap.SetTile(new Vector3Int(x, y, 0), _currentRuleBrush);
        }
    }

    private void PlaceHighestLevelReachedSign(int y, int leftEdgeOfPlatform, int x)
    {
        if (y == _highestFloorReached && x == leftEdgeOfPlatform + 1)
        {
            // for now place flag for highest reached floor on second from left
            GameObject go = Instantiate(HighestLevelReachedAsset, new Vector3(x, y + 1, 0), Quaternion.identity); // +1 for y, because of difference between grid and coord system
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap : MonoBehaviour
{
    [SerializeField]
    int _mapSizeX, _mapSizeY;
    [SerializeField]
    TileType[] _tileTypes; // Contains list of the type of tiles
    [SerializeField]
    Camera _mainCamera;

    int[,] tiles;         // map of tile#s to their type

    private void Start()
    {
        tiles = new int[_mapSizeX, _mapSizeY];
        // Initialize every tile. Default is item at index 0 in _tileTypes i.e. the first in the list.
        for (int x = 0; x < _mapSizeX; x++)
        {
            for (int y = 0; y < _mapSizeY; y++)
            {
                tiles[x, y] = 0;
            }
        }

        GenerateMap();
        // Move the camera to the center of the battlefield
        _mainCamera.transform.position = new Vector3((_mapSizeX - 1) / 2f, (_mapSizeY - 1) / 2f, -10);
    }

    private void GenerateMap()
    {
        for (int x = 0; x < _mapSizeX; x++)
        {
            for (int y = 0; y < _mapSizeY; y++)
            {
                TileType tt = _tileTypes[tiles[x, y]];
                Instantiate(tt.tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
            }
        }
    }
}

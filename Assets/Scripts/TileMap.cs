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
    GameObject _tileEdgesPrefab; // For rendering borders on the tiles
    [SerializeField]
    bool _showTileGrid;
    [SerializeField]
    Camera _mainCamera;

    int[,] tiles;         // map of tile#s to their type

    private void Start()
    {
        GenerateTiles();
        GenerateMapVisuals();
        // Move the camera to the center of the battlefield
        _mainCamera.transform.position = new Vector3((_mapSizeX - 1) / 2f, (_mapSizeY - 1) / 2f, -10);
    }

    private void GenerateTiles()
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

        // TEMP playing with generation logic here. Assuming 8x8 size
        tiles[2, 2] = 1; // Mountains
        tiles[2, 3] = 1; // Mountains
        tiles[2, 4] = 1; // Mountains
        tiles[2, 5] = 1; // Mountains
        tiles[3, 2] = 1; // Mountains
        tiles[3, 3] = 1; // Mountains
        tiles[6, 6] = 2; // Water
        tiles[6, 7] = 2; // Water
        tiles[7, 6] = 2; // Water
        tiles[7, 7] = 2; // Water
    }

    private void GenerateMapVisuals()
    {
        for (int x = 0; x < _mapSizeX; x++)
        {
            for (int y = 0; y < _mapSizeY; y++)
            {
                TileType tt = _tileTypes[tiles[x, y]];

                GameObject tile = Instantiate(tt.tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                if (_showTileGrid)
                {
                    Instantiate(_tileEdgesPrefab, tile.transform);
                }
            }
        }
    }
}

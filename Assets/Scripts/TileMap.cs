using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap : MonoBehaviour
{
    [SerializeField]
    int _mapSizeX, _mapSizeY;
    [SerializeField]
    TileType[] _tileTypes; // Contains list of the type of tiles
    int[,] tiles;         // map of tile#s to their type

    private void Start()
    {
        tiles = new int[_mapSizeX, _mapSizeY];
        // Zero out all the tiles
        for (int x = 0; x < _mapSizeX; x++)
        {
            for (int y = 0; y < _mapSizeY; y++)
            {
                tiles[x, y] = 0;
            }
        }
    }
}

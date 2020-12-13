using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableTile : MonoBehaviour
{
    private TileMap _map;
    private int _tileX, _tileY; // Tile's position in the grid

    public void SetTileMap(TileMap map)
    {
        _map = map;
    }

    public void SetGridPosition(int x, int y)
    {
        _tileX = x;
        _tileY = y;
    }

    private void OnMouseDown()
    {
        Debug.Log("Moving to x: " + _tileX + ", y: " + _tileY);
        _map.GeneratePathTo(_tileX, _tileY);
    }
}

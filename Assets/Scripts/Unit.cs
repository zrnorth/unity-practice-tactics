using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private int _tileX, _tileY;

    public void SetTileMapPosition(int x, int y)
    {
        _tileX = x;
        _tileY = y;
    }
}

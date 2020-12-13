using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileType
{
    public static float IMPASSABLE = -1f;

    public string name;
    public float movementCost; // A movement cost of -1 is completely impassable.
    public GameObject tilePrefab;
}

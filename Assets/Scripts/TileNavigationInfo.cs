using UnityEngine.Tilemaps;

[System.Serializable]
public class TileNavigationInfo
{
    public static float IMPASSABLE = -1f;

    public string name;
    public float movementCost; // A movement cost of -1 is completely impassable.
    public Tile tile;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField]
    private TileMap _map;

    private int _tileX, _tileY;
    private List<Node> _movementPath;

    // Honestly just wanted a chance to try the "out" keyword
    public void GetPosition(out int x, out int y)
    {
        x = _tileX;
        y = _tileY;
    }

    public void SetTileMapPosition(int x, int y)
    {
        _tileX = x;
        _tileY = y;
    }

    public void SetMovementPath(List<Node> path)
    {
        _movementPath = path;
        Debug.Log("New path, size: " + path.Count);
        string str = "";
        foreach (Node n in path)
        {
            str += n.GetPosString();
        }
        Debug.Log(str);
    }

    private void DrawDebugPath()
    {
        if (_movementPath != null)
        {
            for (int curr = 0; curr < _movementPath.Count - 1; curr++)
            {
                Vector3 start = _map.TileCoordToWorldCoord(_movementPath[curr].x, _movementPath[curr].y);
                Vector3 end = _map.TileCoordToWorldCoord(_movementPath[curr + 1].x, _movementPath[curr + 1].y);

                Debug.DrawLine(start, end);
            }
        }
    }
    private void Update()
    {
        DrawDebugPath();
    }
}

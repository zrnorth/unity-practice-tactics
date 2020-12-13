using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField]
    private TileMap _map;
    [SerializeField]
    private bool _canMoveDiagonally; // If true, this unit can move on diagonals, not just NESW.

    private int _tileX, _tileY;
    private List<Node> _movementPath;

    // Honestly just wanted a chance to try the "out" keyword
    public void GetPosition(out int x, out int y)
    {
        x = _tileX;
        y = _tileY;
    }

    public bool CanMoveDiagonally()
    {
        return _canMoveDiagonally;
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

    public void Move()
    {
        if (_movementPath == null) return;
        if (_movementPath.Count <= 1)
        {
            // The first node always contains the current pos.
            // We probably clicked on our exact position, or something.
            _movementPath = null;
            return;
        }

        Node from = _movementPath[0];
        Node to = _movementPath[1];
        _movementPath.RemoveAt(0);

        transform.position = _map.TileCoordToWorldCoord(to.x, to.y);
        _tileX = to.x;
        _tileY = to.y;

        if (_movementPath.Count <= 1)
        {
            // Same check as above, just to potentially clean up 1 turn faster.
            _movementPath = null;
        }
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

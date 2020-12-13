using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    // Modifiers
    [SerializeField]
    private bool _canMoveDiagonally = false; // If true, this unit can move on diagonals, not just NESW.
    [SerializeField]
    private float _movementSpeed = 1; // Number of squares this unit can move per turn

    // References
    [SerializeField]
    private TileMap _map;

    // Class vars
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
        float remainingMovement = _movementSpeed;

        while (remainingMovement > 0)
        {
            if (_movementPath == null) return;
            Node curr = _movementPath[0];
            Node next = _movementPath[1];
            remainingMovement -= _map.CostToEnterNode(curr, next);
            transform.position = _map.WorldCoordsForNode(next);
            _tileX = next.x;
            _tileY = next.y;
            _movementPath.RemoveAt(0);

            // If there's only one node left, we must be at the destination.
            if (_movementPath.Count == 1)
            {
                _movementPath = null;
            }
        }
    }

    private void DrawDebugPath()
    {
        if (_movementPath != null)
        {
            for (int curr = 0; curr < _movementPath.Count - 1; curr++)
            {
                Vector3 start = _map.WorldCoordsForNode(_movementPath[curr]);
                Vector3 end = _map.WorldCoordsForNode(_movementPath[curr + 1]);

                Debug.DrawLine(start, end);
            }
        }
    }

    private void Update()
    {
        DrawDebugPath();
    }
}

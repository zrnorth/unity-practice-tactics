using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Unit : MonoBehaviour
{
    // Modifiers
    [SerializeField]
    private bool _canMoveDiagonally = false; // If true, this unit can move on diagonals, not just NESW.
    [SerializeField]
    private float _movementSpeed = 1; // Number of squares this unit can move per turn
    [SerializeField]
    private Vector2Int _startPos;

    // References
    [SerializeField]
    private Map _map;

    // Class vars
    private int _tileX, _tileY;
    private List<Node> _movementPath;
    private bool _selected;

    private void Start()
    {
        Tilemap tilemap = _map.GetComponent<Tilemap>();
        transform.position = tilemap.CellToWorld(new Vector3Int(_startPos.x, _startPos.y, 0)) + tilemap.tileAnchor;
        // Register with the map so they know we exist
        _map.RegisterUnit(this);
        SetTileMapPosition(_startPos.x, _startPos.y);
        SetSelected(false);
    }

    public void SetSelected(bool isSelected)
    {
        _selected = isSelected;
    }

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
            transform.position = _map.WorldCoordsForCenterOfNode(next);
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

    public Vector3Int GetCellPosition()
    {
        return new Vector3Int(_tileX, _tileY, 0);
    }

    private void DrawDebugPath()
    {
        if (_movementPath != null)
        {
            Vector3Int currPos = new Vector3Int(0, 0, 0);
            Vector3Int nextPos = new Vector3Int(0, 0, 0);
            Tilemap tilemap = _map.GetComponent<Tilemap>();
            for (int curr = 0; curr < _movementPath.Count - 1; curr++)
            {
                currPos.x = _movementPath[curr].x;
                currPos.y = _movementPath[curr].y;
                Vector3 start = tilemap.CellToWorld(currPos) + tilemap.tileAnchor;
                nextPos.x = _movementPath[curr + 1].x;
                nextPos.y = _movementPath[curr + 1].y;
                Vector3 end = tilemap.CellToWorld(nextPos) + tilemap.tileAnchor;

                Debug.DrawLine(start, end);
            }
        }
    }

    private void Update()
    {
        DrawDebugPath();
    }
}

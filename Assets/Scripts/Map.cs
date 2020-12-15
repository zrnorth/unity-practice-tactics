using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    // Game modifiers
    [SerializeField]
    TileNavigationInfo[] _tileNavInfo;          // Contains nav data about some given tiles.
    [SerializeField]
    float _defaultMovementCost = 1f;            // Non-specified tiles have this cost

    // Components
    Tilemap _tileMap;

    // Class vars
    Vector3Int _mapBounds;           // Helper; contains the size of the Tilemap.
    int[,] _tiles;                   // map of tile#s to their type
    Node[,] _graph;                  // Nodes for pathfinding
    private Unit _selectedUnit;
    HashSet<Unit> _unitsOnMap;

    private void Start()
    {
        // TODO: add unit selection
        _tileMap = GetComponent<Tilemap>();
        _mapBounds = _tileMap.cellBounds.size;
        _unitsOnMap = new HashSet<Unit>();
    }

    public void SelectUnit(Unit unit)
    {
        _selectedUnit = unit;
        if (_selectedUnit.CanMoveDiagonally())
        {
            Generate8WayPathfindingGraph();
        }
        else
        {
            Generate4WayPathfindingGraph();
        }
    }


    // No diagonal movement
    private void Generate4WayPathfindingGraph()
    {
        _graph = new Node[_mapBounds.x, _mapBounds.y];
        List<Vector3Int> validPositions = new List<Vector3Int>();
        // Initialize the _graph array
        for (int x = 0; x < _mapBounds.x; x++)
        {
            for (int y = 0; y < _mapBounds.y; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                TileBase tile = _tileMap.GetTile(pos);
                if (tile != null) // If this tile isn't painted we can't path there
                {
                    _graph[x, y] = new Node(x, y);
                    validPositions.Add(pos);
                }
            }
        }

        foreach (Vector3Int pos in validPositions)
        {
            int x = pos.x;
            int y = pos.y;
            List<Node> currNeighbors = _graph[x, y].neighbors;

            if (x > 0 && _graph[x - 1, y] != null)
            {
                currNeighbors.Add(_graph[x - 1, y]);  // West
            }
            if (x < _mapBounds.x - 1 && _graph[x + 1, y] != null)
            {
                currNeighbors.Add(_graph[x + 1, y]);  // East
            }
            if (y > 0 && _graph[x, y - 1] != null)
            {
                currNeighbors.Add(_graph[x, y - 1]);  // South
            }
            if (y < _mapBounds.y - 1 && _graph[x, y + 1] != null)
            {
                currNeighbors.Add(_graph[x, y + 1]);  // North
            }
        }
    }

    // Diagonal movement is allowed
    private void Generate8WayPathfindingGraph()
    {
        _graph = new Node[_mapBounds.x, _mapBounds.y];
        List<Vector3Int> validPositions = new List<Vector3Int>();
        // Initialize the _graph array
        for (int x = 0; x < _mapBounds.x; x++)
        {
            for (int y = 0; y < _mapBounds.y; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                TileBase tile = _tileMap.GetTile(pos);
                if (tile != null) // If this tile isn't painted we can't path there
                {
                    _graph[x, y] = new Node(x, y);
                    validPositions.Add(pos);
                }
            }
        }

        foreach (Vector3Int pos in validPositions)
        {
            int x = pos.x;
            int y = pos.y;
            List<Node> currNeighbors = _graph[x, y].neighbors;

            if (x > 0 && _graph[x - 1, y] != null)
            {
                currNeighbors.Add(_graph[x - 1, y]); // West

                if (y > 0 && _graph[x - 1, y - 1] != null)
                {
                    currNeighbors.Add(_graph[x - 1, y - 1]); // Southwest
                }
                if (y < _mapBounds.y - 1 && _graph[x - 1, y + 1] != null)
                {
                    currNeighbors.Add(_graph[x - 1, y + 1]); // Northwest
                }
            }

            if (x < _mapBounds.x - 1 && _graph[x + 1, y] != null)
            {
                currNeighbors.Add(_graph[x + 1, y]); // East

                if (y > 0 && _graph[x + 1, y - 1] != null)
                {
                    currNeighbors.Add(_graph[x + 1, y - 1]); // Southeast
                }
                if (y < _mapBounds.y - 1 && _graph[x + 1, y + 1] != null)
                {
                    currNeighbors.Add(_graph[x + 1, y + 1]); // Northeast
                }
            }

            if (y > 0 && _graph[x, y - 1] != null)
            {
                currNeighbors.Add(_graph[x, y - 1]); // South
            }
            if (y < _mapBounds.y - 1 && _graph[x, y + 1] != null)
            {
                currNeighbors.Add(_graph[x, y + 1]); // North
            }
        }
    }

    public void RegisterUnit(Unit unit)
    {
        _unitsOnMap.Add(unit);
    }


    // TODO: add unit selection
    // jump directly to (x, y)
    public void TeleportSelectedUnitTo(int x, int y)
    {
        Unit unit = _selectedUnit.GetComponent<Unit>();
        unit.SetTileMapPosition(x, y);
        unit.transform.position = _tileMap.CellToWorld(new Vector3Int(x, y, 0));
    }

    public float CostToEnterTile(int fromX, int fromY, int toX, int toY)
    {
        TileBase tile = _tileMap.GetTile(new Vector3Int(toX, toY, 0));
        TileNavigationInfo tni = Array.Find(_tileNavInfo, t => t.tile.name == tile.name);

        if (tni != null && tni.movementCost == TileNavigationInfo.IMPASSABLE)
        {
            return Mathf.Infinity;
        }

        float cost = tni != null ? tni.movementCost : _defaultMovementCost;
        // We want diagonal moves to be slightly more expensive than NESW moves 
        // because it just looks cleaner to move in cardinal directions.
        if (fromX != toX && fromY != toY)
        {
            cost += 0.001f;
        }
        return cost;
    }

    public float CostToEnterNode(Node from, Node to)
    {
        return CostToEnterTile(from.x, from.y, to.x, to.y);
    }

    public Vector3 WorldCoordsForCenterOfNode(Node n)
    {
        return _tileMap.CellToWorld(new Vector3Int(n.x, n.y, 0)) + _tileMap.tileAnchor;
    }

    // Load the movement path to (x,y) into the currently selected unit
    public void GeneratePathTo(int x, int y)
    {
        var dist = new Dictionary<Node, float>(); // Nodes mapped to their distance from (x,y)
        var prev = new Dictionary<Node, Node>();  // Nodes mapped to the previous Node in the optimal path

        _selectedUnit.GetPosition(out int startX, out int startY);
        Node startNode = _graph[startX, startY];
        Node targetNode = _graph[x, y];
        if (targetNode == null)
        {
            Debug.LogError("There was an error in pathfinding generation. No node data for position: (" +
                x + ", " + y + ")");
            return;
        }

        dist[startNode] = 0;
        prev[startNode] = null;

        // Initialize all nodes to have ∞ distance, a null path, and unvisited
        var unvisited = new List<Node>();
        foreach (Node n in _graph)
        {
            if (n == null) continue; // This tile was not filled, so its not pathable.
            unvisited.Add(n);

            if (n != startNode)
            {
                dist[n] = Mathf.Infinity;
                prev[n] = null;
            }
        }

        while (unvisited.Count > 0)
        {
            // Get the closest remaining node in unvisited set
            Node closest = null;
            foreach (Node possibleClosest in unvisited)
            {
                if (closest == null || dist[possibleClosest] < dist[closest])
                {
                    closest = possibleClosest;
                }
            }

            // If we've found the target, we're done
            if (closest == targetNode)
            {
                break;
            }

            // Recalc the dist/prev for each of this node's neighbors and continue
            unvisited.Remove(closest);
            foreach (Node neighbor in closest.neighbors)
            {
                float alt = dist[closest] + CostToEnterNode(closest, neighbor);
                if (alt < dist[neighbor])
                {
                    dist[neighbor] = alt;
                    prev[neighbor] = closest;
                }
            }
        }

        var path = new List<Node>();

        if (prev[targetNode] != null) // If no route was found, we can't move.
        {
            Node curr = targetNode;
            while (curr != null)
            {
                path.Insert(0, curr); // We are iterating backwards from target, so we prepend
                curr = prev[curr];
            }
        }
        _selectedUnit.SetMovementPath(path);
    }

    // Returns true if a tile is now selected.
    // Returns false if there is no currently selected tile now.
    public bool SelectTile(Vector3Int cell)
    {
        // If we clicked a unit, make it the new selected unit.
        // TODO multiple units on one square might be possible
        foreach (Unit unit in _unitsOnMap)
        {
            if (unit.GetCellPosition() == cell)
            {
                SelectUnit(unit);
                return true;
            }
        }
        // If we clicked outside of the grid, deselect everything
        if (cell.x < 0 ||
            cell.x > _mapBounds.x ||
            cell.y < 0 ||
            cell.y > _mapBounds.y)
        {
            _selectedUnit = null;
            return false;
        }

        // Else we clicked in the grid somewhere, so select the tile.
        // TODO allow tile selections (for later "tile info" page)
        _selectedUnit = null;
        return false;
    }
}
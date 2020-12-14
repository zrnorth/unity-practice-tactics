﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

/*
public class TileMap : MonoBehaviour
{
    // Game modifiers
    [SerializeField]
    int _mapBounds.x, _mapSizeY;
    [SerializeField]
    TileType[] _tileTypes;          // Contains list of the type of tiles
    [SerializeField]
    GameObject _tileEdgesPrefab;    // For rendering borders on the tiles
    [SerializeField]
    bool _showTileGrid;
    [SerializeField]
    Camera _mainCamera;

    // Class vars
    private Unit _selectedUnit;
    int[,] _tiles;                   // map of tile#s to their type
    Node[,] _graph;                  // Nodes for pathfinding


    private void Start()
    {
        // TODO: add unit selection
        _selectedUnit = GameObject.Find("Unit").GetComponent<Unit>();
        TeleportSelectedUnitTo(0, 0);

        if (_selectedUnit.CanMoveDiagonally())
        {
            Generate8WayPathfindingGraph();
        }
        else
        {
            Generate4WayPathfindingGraph();
        }

        GenerateMapVisuals();
        // Move the camera to the center
        _mainCamera.transform.position = new Vector3((_mapSizeX - 1) / 2f, (_mapSizeY - 1) / 2f, -10);
    }

    // No diagonal movement
    private void Generate4WayPathfindingGraph()
    {
        _graph = new Node[_mapSizeX, _mapSizeY];
        // Initialize the _graph array
        for (int x = 0; x < _mapSizeX; x++)
        {
            for (int y = 0; y < _mapSizeY; y++)
            {
                _graph[x, y] = new Node(x, y);
            }
        }

        for (int x = 0; x < _mapSizeX; x++)
        {
            for (int y = 0; y < _mapSizeY; y++)
            {
                List<Node> currNeighbors = _graph[x, y].neighbors;

                if (x > 0)
                {
                    currNeighbors.Add(_graph[x - 1, y]);  // West
                }
                if (x < _mapSizeX - 1)
                {
                    currNeighbors.Add(_graph[x + 1, y]);  // East
                }
                if (y > 0)
                {
                    currNeighbors.Add(_graph[x, y - 1]);  // South
                }
                if (y < _mapSizeY - 1)
                {
                    currNeighbors.Add(_graph[x, y + 1]);  // North
                }

            }
        }
    }

    // Diagonal movement is allowed
    private void Generate8WayPathfindingGraph()
    {
        _graph = new Node[_mapSizeX, _mapSizeY];
        // Initialize the _graph array
        for (int x = 0; x < _mapSizeX; x++)
        {
            for (int y = 0; y < _mapSizeY; y++)
            {
                _graph[x, y] = new Node(x, y);
            }
        }

        for (int x = 0; x < _mapSizeX; x++)
        {
            for (int y = 0; y < _mapSizeY; y++)
            {
                List<Node> currNeighbors = _graph[x, y].neighbors;

                if (x > 0)
                {
                    currNeighbors.Add(_graph[x - 1, y]); // West
                    if (y > 0)
                    {
                        currNeighbors.Add(_graph[x - 1, y - 1]); // Southwest
                    }
                    if (y < _mapSizeY - 1)
                    {
                        currNeighbors.Add(_graph[x - 1, y + 1]); // Northwest
                    }
                }

                if (x < _mapSizeX - 1)
                {
                    currNeighbors.Add(_graph[x + 1, y]); // East
                    if (y > 0)
                    {
                        currNeighbors.Add(_graph[x + 1, y - 1]); // Southeast
                    }
                    if (y < _mapSizeY - 1)
                    {
                        currNeighbors.Add(_graph[x + 1, y + 1]); // Northeast
                    }
                }

                if (y > 0)
                {
                    currNeighbors.Add(_graph[x, y - 1]); // South
                }
                if (y < _mapSizeY - 1)
                {
                    currNeighbors.Add(_graph[x, y + 1]); // North
                }
            }
        }
    }

    private void GenerateMapVisuals()
    {
        for (int x = 0; x < _mapSizeX; x++)
        {
            for (int y = 0; y < _mapSizeY; y++)
            {
                TileType tt = _tileTypes[_tiles[x, y]];

                GameObject tile = Instantiate(tt.tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                ClickableTile ct = tile.GetComponent<ClickableTile>();
                ct.SetGridPosition(x, y);
                ct.SetTileMap(this);
                if (_showTileGrid)
                {
                    Instantiate(_tileEdgesPrefab, tile.transform);
                }
            }
        }
    }

    // Helper to pass in a tile position and return a world coord of the tile position.
    public Vector3 TileCoordToWorldCoord(int x, int y)
    {
        // TODO implement
        return new Vector3(x, y, 0);
    }

    public Vector3 WorldCoordsForNode(Node n)
    {
        return TileCoordToWorldCoord(n.x, n.y);
    }

    // TODO: add unit selection
    // jump directly to (x, y)
    public void TeleportSelectedUnitTo(int x, int y)
    {
        Unit unit = _selectedUnit.GetComponent<Unit>();
        // Set position both in the tilemap and worldmap. These might coincidentally be the same,
        // but lets assume they aren't.
        unit.SetTileMapPosition(x, y);
        unit.transform.position = TileCoordToWorldCoord(x, y);
    }

    public float CostToEnterTile(int fromX, int fromY, int toX, int toY)
    {
        TileType tt = _tileTypes[_tiles[toX, toY]];
        if (tt.movementCost == TileType.IMPASSABLE)
        {
            return Mathf.Infinity;
        }
        float cost = tt.movementCost;
        // We want diagonal moves to be slightly more expensive than NESW moves 
        // because it just looks cleaner to move in cardinal directions.
        if (fromX != toX && fromY != toY)
        {
            float smallestCostInTileTypes = Mathf.Max(_tileTypes.Min(t => t.movementCost), 0.01f);
            cost += smallestCostInTileTypes * 0.01f;
        }
        return cost;
    }

    public float CostToEnterNode(Node from, Node to)
    {
        return CostToEnterTile(from.x, from.y, to.x, to.y);
    }

    // Load the movement path to (x,y) into the currently selected unit
    public void GeneratePathTo(int x, int y)
    {
        var dist = new Dictionary<Node, float>(); // Nodes mapped to their distance from (x,y)
        var prev = new Dictionary<Node, Node>();  // Nodes mapped to the previous Node in the optimal path

        _selectedUnit.GetPosition(out int startX, out int startY);
        Node startNode = _graph[startX, startY];
        Node targetNode = _graph[x, y];

        dist[startNode] = 0;
        prev[startNode] = null;

        // Initialize all nodes to have ∞ distance, a null path, and unvisited
        var unvisited = new List<Node>();
        foreach (Node n in _graph)
        {
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
}
*/

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
    private Unit _selectedUnit;
    int[,] _tiles;                   // map of tile#s to their type
    Node[,] _graph;                  // Nodes for pathfinding


    private void Start()
    {
        // TODO: add unit selection
        _tileMap = GetComponent<Tilemap>();
        _selectedUnit = GameObject.Find("Unit").GetComponent<Unit>();
        _mapBounds = _tileMap.cellBounds.size;

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
        TileNavigationInfo tni = Array.Find(_tileNavInfo, t => t.name == tile.name);

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

    private void OnMouseDown()
    {
        // Get the tile the player clicked
        Vector3Int clickedTile = _tileMap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        Debug.Log("Moving to x: " + clickedTile.x + ", y: " + clickedTile.y);
        GeneratePathTo(clickedTile.x, clickedTile.y);
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileMap : MonoBehaviour
{
    // Game modifiers
    [SerializeField]
    int _mapSizeX, _mapSizeY;
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
        GenerateTiles();
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

    private void GenerateTiles()
    {
        _tiles = new int[_mapSizeX, _mapSizeY];
        // Initialize every tile. Default is item at index 0 in _tileTypes i.e. the first in the list.
        for (int x = 0; x < _mapSizeX; x++)
        {
            for (int y = 0; y < _mapSizeY; y++)
            {
                _tiles[x, y] = 0;
            }
        }

        // TEMP making a custom map here. Assuming 8x8 size
        _tiles[4, 5] = 1; // Mountains
        _tiles[5, 5] = 1; // Mountains
        _tiles[6, 3] = 1; // Mountains
        _tiles[6, 4] = 1; // Mountains
        _tiles[6, 5] = 1; // Mountains
        _tiles[1, 1] = 2; // Water
        _tiles[1, 2] = 2; // Water
        _tiles[1, 3] = 2; // Water
        _tiles[2, 3] = 2; // Water
        _tiles[2, 1] = 2; // Water
        _tiles[3, 1] = 2; // Water
        _tiles[3, 2] = 2; // Water
        _tiles[6, 6] = 2; // Water
        _tiles[6, 7] = 2; // Water
        _tiles[7, 6] = 2; // Water
        _tiles[7, 7] = 2; // Water

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

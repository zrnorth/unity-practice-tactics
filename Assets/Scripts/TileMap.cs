using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// For pathfinding
public class Node
{
    public int x, y;
    public List<Node> neighbors; // Not an array lmao
    public Node(int x, int y)
    {
        this.x = x;
        this.y = y;
        neighbors = new List<Node>();
    }

    public float DistanceTo(Node n)
    {
        // Could use taxicab distance here, not sure what's better.
        // Eventually might need to scale this based on stuff like
        // swamps taking 2 moves to get through instead of 1
        return Vector2.Distance(
            new Vector2(this.x, this.y),
            new Vector2(n.x, n.y));
    }
    public string GetPosString()
    {
        return "(" + x + ", " + y + ")";
    }
    public override string ToString()
    {
        string str = GetPosString() + " -> [";
        foreach (Node neighbor in neighbors)
        {
            str += neighbor.GetPosString() + ", ";
        }
        str += "]";
        return str;
    }
}

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
    int[,] tiles;                   // map of tile#s to their type
    Node[,] _graph;                 // Nodes for pathfinding


    private void Start()
    {
        GenerateTiles();
        GeneratePathfindingGraph();
        GenerateMapVisuals();
        // Move the camera to the center of the battlefield
        _mainCamera.transform.position = new Vector3((_mapSizeX - 1) / 2f, (_mapSizeY - 1) / 2f, -10);

        // TODO: add unit selection
        _selectedUnit = GameObject.Find("Unit").GetComponent<Unit>();
        TeleportSelectedUnitTo(0, 0);
    }

    private void GenerateTiles()
    {
        tiles = new int[_mapSizeX, _mapSizeY];
        // Initialize every tile. Default is item at index 0 in _tileTypes i.e. the first in the list.
        for (int x = 0; x < _mapSizeX; x++)
        {
            for (int y = 0; y < _mapSizeY; y++)
            {
                tiles[x, y] = 0;
            }
        }

        // TEMP playing with generation logic here. Assuming 8x8 size
        tiles[2, 2] = 1; // Mountains
        tiles[2, 3] = 1; // Mountains
        tiles[2, 4] = 1; // Mountains
        tiles[2, 5] = 1; // Mountains
        tiles[3, 2] = 1; // Mountains
        tiles[3, 3] = 1; // Mountains
        tiles[6, 6] = 2; // Water
        tiles[6, 7] = 2; // Water
        tiles[7, 6] = 2; // Water
        tiles[7, 7] = 2; // Water
    }

    private void GeneratePathfindingGraph()
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
                Node curr = _graph[x, y];
                // No diagonal movement
                if (y < _mapSizeY - 1)
                {
                    curr.neighbors.Add(_graph[x, y + 1]);
                }
                if (y > 0)
                {
                    curr.neighbors.Add(_graph[x, y - 1]);
                }
                if (x < _mapSizeX - 1)
                {
                    curr.neighbors.Add(_graph[x + 1, y]);
                }
                if (x > 0)
                {
                    curr.neighbors.Add(_graph[x - 1, y]);
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
                TileType tt = _tileTypes[tiles[x, y]];

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
                float alt = dist[closest] + closest.DistanceTo(neighbor);
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

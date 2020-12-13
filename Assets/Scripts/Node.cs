using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Node
{
    public int x, y;
    public List<Node> neighbors;
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

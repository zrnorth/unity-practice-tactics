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

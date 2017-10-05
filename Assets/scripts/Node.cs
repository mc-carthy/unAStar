﻿using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool walkable;
    public Vector3 worldPos;
    public int gridX, gridY;

    public int gCost;
    public int hCost;
    public int fCost { get { return gCost + hCost; } }
    public int HeapIndex { get { return heapIndex; } set { heapIndex = value; } }
    public Node parent;
    
    private int heapIndex;

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPos = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int CompareTo(Node other)
    {
        int compare = fCost.CompareTo(other.fCost);

        if (compare == 0)
        {
            compare = hCost.CompareTo(other.hCost);
        }

        return -compare;
    }
}

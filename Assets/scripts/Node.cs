﻿using UnityEngine;

public class Node
{
    public bool walkable;
    public Vector3 worldPos;
    public int gridX, gridY;

    public int gCost;
    public int hCost;
    public int fCost { get { return gCost + hCost; } }
    public Node parent;

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPos = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }
}

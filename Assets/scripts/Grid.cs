﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public LayerMask obstacleLayer;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    
    private Node[,] grid;
    private float nodeDiameter;
    private int gridSizeX, gridSizeY;

    private void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        CreateGrid();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null)
        {
            foreach(Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                Gizmos.DrawWireCube(n.worldPos, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }

    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - (Vector3.right * gridSizeX / 2) - (Vector3.forward * gridSizeY / 2);

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = 
                    worldBottomLeft + 
                    (Vector3.right * (x * nodeDiameter + nodeRadius)) + 
                    (Vector3.forward * (y * nodeDiameter + nodeRadius));
                
                bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius, obstacleLayer);

                grid[x, y] = new Node(walkable, worldPoint);
            }
        }
    }
}

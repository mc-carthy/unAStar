using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// This script contains the a-star algorithm that the grid class will call
// Both need to be attached to the same game object in order to function

[RequireComponent(typeof(Grid))]
[RequireComponent(typeof(PathRequestManager))]
public class Pathfinding : MonoBehaviour {


    private PathRequestManager pathRequestManager;
    private Grid grid;

    private void Awake()
    {
        grid = GetComponent<Grid>();
        pathRequestManager = GetComponent<PathRequestManager>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 endPos)
    {
        StartCoroutine(FindAStarPath(startPos, endPos));
    }

    private IEnumerator FindAStarPath(Vector3 startPos, Vector3 endPos)
    {
        Node startNode = grid.GetNodeFromWorldPoint(startPos);
        Node endNode = grid.GetNodeFromWorldPoint(endPos);

        if (startNode.walkable && endNode.walkable)
        {
            StartCoroutine(FindAStarPath(startNode, endNode));
        }

        yield return null;
    }

    private IEnumerator FindAStarPath(Node startNode, Node endNode)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        if (startNode.walkable && endNode.walkable)
        {
            // Unexplored nodes neighbouring to explored nodes
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            // Explored nodes
            HashSet<Node> closedSet = new HashSet<Node>();

            openSet.Add(startNode);

            while(openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveTop();
                closedSet.Add(currentNode);

                if (currentNode == endNode)
                {
                    sw.Stop();
                    print("Path found in " + sw.ElapsedMilliseconds + "ms.");

                    pathSuccess = true;
                    break;
                }

                foreach(Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, endNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                        else
                        {
                            openSet.UpdateItem(neighbour);
                        }
                    }

                }
            }
        }

        yield return null;
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, endNode);
        }
        pathRequestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    private Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        Vector3[] waypoints = SimplifyPath(path);

        Array.Reverse(waypoints);

        return waypoints;

    }

    private Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 dirOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 dirNew = new Vector2(path[i-1].gridX - path[i].gridX, path[i-1].gridY - path[i].gridY);
            if (dirNew != dirOld)
            {
                waypoints.Add(path[i].worldPos);
            }
            dirOld = dirNew;
        }

        return waypoints.ToArray();
    }

    private int GetDistance(Node a, Node b)
    {
        int dx = Mathf.Abs(a.gridX - b.gridX);
        int dy = Mathf.Abs(a.gridY - b.gridY);

        int min = Mathf.Min(dx, dy);
        int max = Mathf.Max(dx, dy);

        return (14 * min) + 10 * (max - min);
    }
}

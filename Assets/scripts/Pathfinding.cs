using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// This script contains the a-star algorithm that the grid class will call
// Both need to be attached to the same game object in order to function

[RequireComponent(typeof(Grid))]
public class Pathfinding : MonoBehaviour {

    public Transform seeker, target;

    private Grid grid;

    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FindAStarPath(seeker.position, target.position);
        }
    }

    private void FindAStarPath(Vector3 startPos, Vector3 endPos)
    {
        Node startNode = grid.GetNodeFromWorldPoint(startPos);
        Node endNode = grid.GetNodeFromWorldPoint(endPos);

        FindAStarPath(startNode, endNode);
    }

    private void FindAStarPath(Node startNode, Node endNode)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();


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


                RetracePath(startNode, endNode);
                return;
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

    private void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        grid.path = path;
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

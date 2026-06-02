using UnityEngine;
using System.Collections.Generic;

public static class ThetaStar
{
    public static List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos, LayerMask obstacleMask)
    {
        Node startNode = GridManager.Instance.NodeFromWorldPoint(startPos);
        Node targetNode = GridManager.Instance.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbor in GridManager.Instance.GetNeighbors(currentNode))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor)) continue;

                if (currentNode.parent != null && InLineOfSight(currentNode.parent.worldPosition, neighbor.worldPosition, obstacleMask))
                {
                    float newMovementCostToNeighbor = currentNode.parent.gCost + Vector3.Distance(currentNode.parent.worldPosition, neighbor.worldPosition);
                    if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                    {
                        neighbor.gCost = newMovementCostToNeighbor;
                        neighbor.hCost = Vector3.Distance(neighbor.worldPosition, targetNode.worldPosition);
                        neighbor.parent = currentNode.parent; // Salteamos el nodo actual, conectamos directo al abuelo
                        if (!openSet.Contains(neighbor)) openSet.Add(neighbor);
                    }
                }
                else // Si hay un obstáculo, nos comportamos como un A* común nodo a nodo
                {
                    float newMovementCostToNeighbor = currentNode.gCost + Vector3.Distance(currentNode.worldPosition, neighbor.worldPosition);
                    if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                    {
                        neighbor.gCost = newMovementCostToNeighbor;
                        neighbor.hCost = Vector3.Distance(neighbor.worldPosition, targetNode.worldPosition);
                        neighbor.parent = currentNode;
                        if (!openSet.Contains(neighbor)) openSet.Add(neighbor);
                    }
                }
            }
        }
        return null;
    }

    private static bool InLineOfSight(Vector3 start, Vector3 end, LayerMask mask)
    {
        Vector3 dir = end - start;
        return !Physics.Raycast(start, dir.normalized, dir.magnitude, mask);
    }

    private static List<Vector3> RetracePath(Node startNode, Node endNode)
    {
        List<Vector3> path = new List<Vector3>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.worldPosition);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }
}
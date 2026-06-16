using UnityEngine;
using System.Collections.Generic;

public static class ThetaStar
{
    // Theta* sobre el grid de GridManager. devuelve la ruta en posiciones de mundo (o null).
    // gScore/parents son locales a cada llamada: los Node no guardan costos, asi no se
    // contaminan busquedas seguidas (era el bug del enfoque anterior).
    public static List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos, LayerMask obstacleMask, int maxExpansions = 2000)
    {
        GridManager grid = GridManager.Instance;
        if (grid == null) return null;

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);
        if (startNode == null || targetNode == null || !startNode.walkable) return null;

        var gScore = new Dictionary<Node, float> { [startNode] = 0f };
        var parents = new Dictionary<Node, Node>();
        var closedSet = new HashSet<Node>();
        var pending = new PriorityQueue<Node>();

        pending.Enqueue(startNode, Heuristic(startNode, targetNode));

        int expansions = 0;
        while (!pending.IsEmpty)
        {
            if (++expansions > maxExpansions) break; // watchdog: evita picos si el destino es inalcanzable

            Node current = pending.Dequeue();
            // la PriorityQueue no tiene decrease-key: puede haber duplicados viejos.
            // si el nodo ya se expandio, lo salteo
            if (closedSet.Contains(current)) continue;
            closedSet.Add(current);

            if (current == targetNode) return RetracePath(parents, startNode, targetNode);

            foreach (Node neighbor in grid.GetNeighbors(current))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor)) continue;

                // corazon de Theta*: si el "abuelo" ve directo al vecino, lo conecto a el
                // y salteo el nodo actual (rutas diagonales, sin zig-zag por la grilla)
                Node from = current;
                if (parents.TryGetValue(current, out Node grandParent) &&
                    InLineOfSight(grandParent.worldPosition, neighbor.worldPosition, obstacleMask))
                {
                    from = grandParent;
                }

                float tentativeG = gScore[from] + Vector3.Distance(from.worldPosition, neighbor.worldPosition);
                if (gScore.TryGetValue(neighbor, out float existing) && tentativeG >= existing) continue;

                gScore[neighbor] = tentativeG;
                parents[neighbor] = from;
                pending.Enqueue(neighbor, tentativeG + Heuristic(neighbor, targetNode));
            }
        }
        return null;
    }

    private static float Heuristic(Node a, Node target)
    {
        return Vector3.Distance(a.worldPosition, target.worldPosition);
    }

    private static bool InLineOfSight(Vector3 start, Vector3 end, LayerMask mask)
    {
        Vector3 dir = end - start;
        float dist = dir.magnitude;
        if (dist <= Mathf.Epsilon) return true;
        return !Physics.Raycast(start, dir / dist, dist, mask);
    }

    private static List<Vector3> RetracePath(Dictionary<Node, Node> parents, Node startNode, Node endNode)
    {
        List<Vector3> path = new List<Vector3>();
        Node current = endNode;

        while (current != startNode)
        {
            path.Add(current.worldPosition);
            if (!parents.TryGetValue(current, out current)) break; // seguridad: cadena rota
        }
        path.Reverse();
        return path;
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Dijkstra
{
    public static List<Vector2Int> FindShortestPath(List<Vector2Int> grid, Vector2Int start, Vector2Int end)
    {
        // Dictionary to store the cost to reach each node
        Dictionary<Vector2Int, float> costSoFar = new Dictionary<Vector2Int, float>();

        // Dictionary to store the node from which we came to each node
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        // Priority queue to store nodes to visit, sorted by cost
        PriorityQueue<Vector2Int> frontier = new PriorityQueue<Vector2Int>();

        // Initialize start node
        costSoFar[start] = 0;
        frontier.Enqueue(start, 0);

        // Explore the grid
        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();

            // Check if we've reached the end
            if (current.x == 0)
            {
                return ReconstructPath(cameFrom, current);
            }

            // Get neighbors
            List<Vector2Int> neighbors = GetNeighbors(grid, current);

            foreach (Vector2Int next in neighbors)
            {
                float newCost = costSoFar[current] + 1; // Assuming uniform cost of 1

                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    float priority = newCost + Heuristic(next, end);
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }

        // No path found
        Debug.LogWarning("No path found to the end of the grid.");
        return new List<Vector2Int> { start };
    }

    // Get valid neighbors of a cell
    static List<Vector2Int> GetNeighbors(List<Vector2Int> grid, Vector2Int current)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        Vector2Int[] directions = { Vector2Int.left, Vector2Int.up, Vector2Int.down }; // Possible move directions

        foreach (Vector2Int dir in directions)
        {
            Vector2Int neighbor = current + dir;
            if (grid.Contains(neighbor))
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    // A simple Manhattan distance heuristic for A* (optional, can improve efficiency)
    static float Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    // Reconstruct the path from the cameFrom dictionary
    static List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        while (cameFrom.ContainsKey(current))
        {
            path.Add(current);
            current = cameFrom[current];
        }
        path.Reverse(); // Reverse to get path from start to end
        return path;
    }
}

// Simple priority queue implementation (you might want to use a more robust one)
public class PriorityQueue<T>
{
    private List<KeyValuePair<T, float>> elements = new List<KeyValuePair<T, float>>();

    public int Count
    {
        get { return elements.Count; }
    }

    public void Enqueue(T item, float priority)
    {
        elements.Add(new KeyValuePair<T, float>(item, priority));
    }

    public T Dequeue()
    {
        int bestIndex = 0;

        for (int i = 0; i < elements.Count; i++)
        {
            if (elements[i].Value < elements[bestIndex].Value)
            {
                bestIndex = i;
            }
        }

        T bestItem = elements[bestIndex].Key;
        elements.RemoveAt(bestIndex);
        return bestItem;
    }
}

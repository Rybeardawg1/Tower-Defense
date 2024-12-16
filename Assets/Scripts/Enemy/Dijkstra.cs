using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Dijkstra
{
    public static List<Vector2Int> FindShortestPath(List<Vector2Int> grid, Vector2Int start, Vector2Int end)
    {
        Dictionary<Vector2Int, float> costSoFar = new Dictionary<Vector2Int, float>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        PriorityQueue<Vector2Int> frontier = new PriorityQueue<Vector2Int>();

        costSoFar[start] = 0;
        frontier.Enqueue(start, 0);

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();

            if (current.x == 0)
            {
                return ReconstructPath(cameFrom, current);
            }

            List<Vector2Int> neighbors = GetNeighbors(grid, current);

            foreach (Vector2Int next in neighbors)
            {
                float newCost = costSoFar[current] + 1;

                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    float priority = newCost + Heuristic(next, end);
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }

        Debug.LogWarning("No path found to the end of the grid.");
        return new List<Vector2Int> { start };
    }

    static List<Vector2Int> GetNeighbors(List<Vector2Int> grid, Vector2Int current)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        Vector2Int[] directions = { Vector2Int.left, Vector2Int.up, Vector2Int.down };

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

    static float Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    static List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        while (cameFrom.ContainsKey(current))
        {
            path.Add(current);
            current = cameFrom[current];
        }
        path.Reverse();
        return path;
    }
}

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

        // Randomly choose a different path for the enemy so it surprises the player
        float random_chance = 0.5f;

        if (Random.value < random_chance)
        {
            Debug.Log("Randomly choosing a different path for the enemy.");
            bestIndex = Random.Range(0, elements.Count);
        }

        T bestItem = elements[bestIndex].Key;
        elements.RemoveAt(bestIndex);
        return bestItem;
    }
}

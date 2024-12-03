using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public GameObject cellPrefab; // Assign a cube or custom cell prefab in the Inspector
    public int gridSizeX = 16;
    public int gridSizeZ = 16;
    public float cellSize = 1f;

    //private List<Vector2Int> pathPositions;
    public List<Vector2Int> pathPositions { get; private set; } // Expose the path positions

    void Start()
    {
        GenerateGrid();
        GeneratePath();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                Vector3 cellPosition = new Vector3(x * cellSize, 0, z * cellSize);
                GameObject cell = Instantiate(cellPrefab, cellPosition, Quaternion.identity, transform);
                cell.name = $"Cell_{x}_{z}";
                cell.GetComponent<Renderer>().material.color = Color.green; // Default tile color
            }
        }
    }

    void GeneratePath()
    {
        if (pathPositions == null)
        {
            pathPositions = new List<Vector2Int>();
        }
        else
        {
            pathPositions.Clear(); // Clear the list for a fresh generation
        }

        Debug.Log("Starting path generation...");

        // Start path at random position on the left edge
        Vector2Int currentPosition = new Vector2Int(0, Random.Range(0, gridSizeZ));
        pathPositions.Add(currentPosition);

        while (currentPosition.x < gridSizeX - 1) // Until we reach the other side
        {
            List<Vector2Int> nextSteps = GetValidSteps(currentPosition);
            if (nextSteps.Count == 0)
            {
                Debug.LogError("Path generation terminated early due to no valid steps.");
                break;
            }

            currentPosition = nextSteps[Random.Range(0, nextSteps.Count)];
            pathPositions.Add(currentPosition);
        }

        // Color the path green
        foreach (Vector2Int pos in pathPositions)
        {
            Transform cell = transform.Find($"Cell_{pos.x}_{pos.y}");
            if (cell != null)
            {
                //cell.GetComponent<Renderer>().material.color = Color.green; // Path tile color
                // color it dirt brown
                cell.GetComponent<Renderer>().material.color = new Color(0.5f, 0.35f, 0.05f);

            }
        }

        if (pathPositions.Count == 0)
        {
            Debug.LogError("Path generation failed! No valid path created.");
        }
        else
        {
            Debug.Log($"Path generated with {pathPositions.Count} positions.");
        }
    }

    List<Vector2Int> GetValidSteps(Vector2Int currentPosition)
    {
        List<Vector2Int> steps = new List<Vector2Int>();

        // Check moves in four cardinal directions
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 0),  // Move right
            new Vector2Int(0, 1),  // Move up
            new Vector2Int(0, -1), // Move down
        };

        foreach (Vector2Int direction in directions)
        {
            Vector2Int newPos = currentPosition + direction;
            if (newPos.x >= 0 && newPos.x < gridSizeX && newPos.y >= 0 && newPos.y < gridSizeZ)
            {
                // Ensure we don't backtrack onto an existing path position
                if (!pathPositions.Contains(newPos))
                {
                    steps.Add(newPos);
                }
            }
        }

        return steps;
    }
}

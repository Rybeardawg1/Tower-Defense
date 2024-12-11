using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GridGenerator : MonoBehaviour
{
    public GameObject cellPrefab; // Assign a cube or custom cell prefab in the Inspector
    public GameObject towerPrefab;
    public int gridSizeX = 16;
    public int gridSizeZ = 16;
    public float cellSize = 1f;

    //private List<Vector2Int> pathPositions;
    public List<Vector2Int> pathPositions { get; private set; } // Expose the path positions
    public List<Vector2Int> towerPositions { get; private set; } // Expose the path positions
    private Dictionary<Vector2Int, GameObject> gridCells = new Dictionary<Vector2Int, GameObject>(); // To track cells for coloring
    public List<Vector2Int> towerPlacementZones { get; private set; }


    void Start()
    {
        GenerateGrid();
        GeneratePath();

    }

    // void GenerateGrid()
    // {
    //     for (int x = 0; x < gridSizeX; x++)
    //     {
    //         for (int z = 0; z < gridSizeZ; z++)
    //         {
    //             Vector3 cellPosition = new Vector3(x * cellSize, 0, z * cellSize);
    //             GameObject cell = Instantiate(cellPrefab, cellPosition, Quaternion.identity, transform);
    //             cell.name = $"Cell_{x}_{z}";
    //             cell.GetComponent<Renderer>().material.color = Color.green; // Default tile color
    //         }
    //     }
    // }

    void GenerateGrid()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                bool isTower = ShouldGenerateTower();
                Vector3 cellPosition = new Vector3(x * cellSize, 0, z * cellSize);

                if (!isTower)
                {
                    GameObject cell = Instantiate(cellPrefab, cellPosition, Quaternion.identity, transform);
                    cell.name = $"Cell_{x}_{z}";
                    cell.tag = "GreenCell";
                    cell.GetComponent<Renderer>().material.color = Color.green; // Default tile color

                    // Track cells in a dictionary for easy recoloring
                    gridCells[new Vector2Int(x, z)] = cell;
                }
                else
                {
                    GameObject cell = Instantiate(towerPrefab, cellPosition, Quaternion.identity, transform);
                    cell.tag = "TurretCell";
                    cell.transform.localScale = new Vector3((float)0.1, (float)0.1, (float)0.1);
                    cell.name = $"Cell_{x}_{z}";

                    // Track cells in a dictionary for easy recoloring
                    gridCells[new Vector2Int(x, z)] = cell;
                }

            }
        }

        

    }

    static bool ShouldGenerateTower()
    {
        double probability = 0.02;

        System.Random random = new System.Random();
        double randomValue = random.NextDouble(); // Generates a number between 0.0 and 1.0
        return randomValue < probability;
    }


    // public void InitializeGrid()
    // {
    //     foreach (Transform child in transform)
    //     {
    //         Destroy(child.gameObject); // Clear previous cells
    //     }

    //     GenerateGrid();
    //     GeneratePath();
    // }


    public void InitializeGrid()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject); // Clear previous cells
        }

        gridCells.Clear();
        GenerateGrid();
        GeneratePath();
        foreach (Vector2Int pos in pathPositions)
        {
            if (gridCells.TryGetValue(pos, out GameObject cell))
            {
                cell.GetComponent<Renderer>().material.color = Color.blue; // Blue for the path
            }
        }
        VisualizeTowerZones();

    }

    void VisualizeTowerZones()
    {
         foreach (Vector2Int towerCell in towerPlacementZones)
    {
        if (gridCells.TryGetValue(towerCell, out GameObject cell))
        {
            Renderer renderer = cell.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.yellow; // Yellow for tower zones
            }
            else
            {
                Debug.LogWarning($"No Renderer found on cell {cell.name}");
            }
        }
    }
    }


    // void GeneratePath()
    // {
    //     if (pathPositions == null)
    //     {
    //         pathPositions = new List<Vector2Int>();
    //     }
    //     else
    //     {
    //         pathPositions.Clear(); // Clear the list for a fresh generation
    //     }

    //     // Log grid size
    //     Debug.Log($"Generating path on grid {gridSizeX}x{gridSizeZ}");


    //     // Start path at random position on the left edge
    //     Vector2Int currentPosition = new Vector2Int(0, Random.Range(0, gridSizeZ));
    //     pathPositions.Add(currentPosition);

    //     while (currentPosition.x < gridSizeX - 1) // Until we reach the other side
    //     {
    //         List<Vector2Int> nextSteps = GetValidSteps(currentPosition);
    //         if (nextSteps.Count == 0)
    //         {
    //             Debug.LogError("Path generation terminated early due to no valid steps.");
    //             break;
    //         }

    //         currentPosition = nextSteps[Random.Range(0, nextSteps.Count)];
    //         pathPositions.Add(currentPosition);
    //     }

    //     // Color the path green
    //     foreach (Vector2Int pos in pathPositions)
    //     {
    //         Transform cell = transform.Find($"Cell_{pos.x}_{pos.y}");
    //         if (cell != null)
    //         {
    //             //cell.GetComponent<Renderer>().material.color = Color.green; // Path tile color
    //             // color it dirt brown
    //             cell.GetComponent<Renderer>().material.color = new Color(0.5f, 0.35f, 0.05f);

    //         }
    //     }

    //     if (pathPositions.Count == 0)
    //     {
    //         Debug.LogError("Path generation failed! No valid path created.");
    //     }
    //     else
    //     {
    //         Debug.Log($"Path generated with {pathPositions.Count} positions.");
    //     }
    // }


    void GeneratePath()
    {
        pathPositions = new List<Vector2Int>();
        towerPlacementZones = new List<Vector2Int>();

        // Start path at random position on the left edge
        Vector2Int currentPosition = new Vector2Int(0, Random.Range(0, gridSizeZ));
        pathPositions.Add(currentPosition);


        while (currentPosition.x < gridSizeX - 1) // Until we reach the other side
        {
            List<Vector2Int> nextSteps = GetValidSteps(currentPosition);
            if (nextSteps.Count == 0)
            {
                Debug.LogError("No valid steps available. Path generation failed.");
                break;
            }

            currentPosition = nextSteps[Random.Range(0, nextSteps.Count)];
            pathPositions.Add(currentPosition);
        }

        // Add adjacent cells to towerPlacementZones
        foreach (Vector2Int pos in pathPositions)
        {
            AddAdjacentCells(pos);
        }

        Debug.Log($"Path generated with {pathPositions.Count} positions.");
        Debug.Log($"Tower placement zones: {towerPlacementZones.Count}");
    }

    void AddAdjacentCells(Vector2Int position)
    {
        Vector2Int[] directions = {
        new Vector2Int(1, 0),  // Right
        new Vector2Int(-1, 0), // Left
        new Vector2Int(0, 1),  // Up
        new Vector2Int(0, -1)  // Down
    };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int adjacentCell = position + dir;

            // Add sparsity by skipping some cells randomly
            if (Random.Range(0, 4) == 0) 
                continue;

            if (IsValidCell(adjacentCell) &&
                !pathPositions.Contains(adjacentCell) &&
                !towerPlacementZones.Contains(adjacentCell))
            {
                towerPlacementZones.Add(adjacentCell);
            }
        }


    }


    bool IsValidCell(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < gridSizeX && cell.y >= 0 && cell.y < gridSizeZ;
    }





    // List<Vector2Int> GetValidSteps(Vector2Int currentPosition)
    // {
    //     List<Vector2Int> steps = new List<Vector2Int>();

    //     // Check moves in four cardinal directions
    //     Vector2Int[] directions = new Vector2Int[]
    //     {
    //         new Vector2Int(1, 0),  // Move right
    //         new Vector2Int(0, 1),  // Move up
    //         new Vector2Int(0, -1), // Move down
    //     };

    //     foreach (Vector2Int direction in directions)
    //     {
    //         Vector2Int newPos = currentPosition + direction;
    //         if (newPos.x >= 0 && newPos.x < gridSizeX && newPos.y >= 0 && newPos.y < gridSizeZ)
    //         {
    //             // Ensure we don't backtrack onto an existing path position
    //             if (!pathPositions.Contains(newPos))
    //             {
    //                 steps.Add(newPos);
    //             }
    //         }
    //     }

    //     return steps;
    // }
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

            if (gridCells.TryGetValue(newPos, out GameObject cell))
            {
                if (cell.tag == "TurretCell")
                {
                    continue;
                }
            }

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

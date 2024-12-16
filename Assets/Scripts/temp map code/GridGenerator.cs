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


    public float pathLengthFactor = 0.5f; // 0 = shortest, 1 = longest
    public int minStraightLength = 3; // Minimum length of a straight path segment
    public int maxStraightLength = 8; // Maximum length of a straight path segment
    public int edgeAvoidance = 2; // Minimum distance from the edges of the grid

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


    void GenerateGrid()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                //bool isTower = ShouldGenerateTower();
                Vector3 cellPosition = new Vector3(x * cellSize, 0, z * cellSize);

                //if (!isTower)
                //{
                GameObject cell = Instantiate(cellPrefab, cellPosition, Quaternion.identity, transform);
                cell.name = $"Cell_{x}_{z}";
                cell.tag = "GreenCell";
                cell.GetComponent<Renderer>().material.color = Color.green; // Default tile color

                // Track cells in a dictionary for easy recoloring
                gridCells[new Vector2Int(x, z)] = cell;
                //}
                //else
                //{
                //    GameObject cell = Instantiate(towerPrefab, cellPosition, Quaternion.identity, transform);
                //    cell.tag = "TurretCell";
                //    cell.transform.localScale = new Vector3((float)0.1, (float)0.1, (float)0.1);
                //    cell.name = $"Cell_{x}_{z}";

                //    // Track cells in a dictionary for easy recoloring
                //    gridCells[new Vector2Int(x, z)] = cell;
                //}

            }
        }
    }

    //static bool ShouldGenerateTower()
    //{
    //    double probability = 0.02;

    //    System.Random random = new System.Random();
    //    double randomValue = random.NextDouble(); // Generates a number between 0.0 and 1.0
    //    return randomValue < probability;
    //}
    //static bool ShouldGenerateTower()
    //{
    //    double probability = 0.02;

    //    System.Random random = new System.Random();
    //    double randomValue = random.NextDouble(); // Generates a number between 0.0 and 1.0
    //    return randomValue < probability;
    //}


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
                //cell.GetComponent<Renderer>().material.color = Color.blue; // Blue for the path
                //cell.GetComponent<Renderer>().material.color = new Color(0.5f, 0.35f, 0.05f);
                cell.GetComponent<Renderer>().material.color = new Color(0.4f, 0.25f, 0.05f);
            }
        }
        VisualizeTowerZones();
    }

    //void VisualizeTowerZones()
    //{
    //    foreach (Vector2Int towerCell in towerPlacementZones)
    //    {
    //        if (gridCells.TryGetValue(towerCell, out GameObject cell))
    //        {
    //            //cell.GetComponent<Renderer>().material.color = Color.yellow; // Yellow for tower zones
    //            cell.GetComponent<Renderer>().material.color = new Color(0.0f, 0.5f, 0.0f);
    //        }
    //    }
    //}

    void VisualizeTowerZones()
    {
         foreach (Vector2Int towerCell in towerPlacementZones)
    {
        if (gridCells.TryGetValue(towerCell, out GameObject cell))
        {
            Renderer renderer = cell.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(0.0f, 0.5f, 0.0f); // Yellow for tower zones
            }
            else
            {

                ////Renderer renderer = cell.GetComponent<Renderer>();
                //if (renderer != null)
                //{
                //    //renderer.material.color = Color.yellow; // Yellow for tower zones
                //    renderer.material.color = new Color(0.0f, 0.5f, 0.0f);
                //}
                //else
                //{
                Debug.LogWarning($"No Renderer found on cell {cell.name}");
                //}
            }
        }
    }
    }


    //void GeneratePath()
    //{
    //    pathPositions = new List<Vector2Int>();
    //    towerPlacementZones = new List<Vector2Int>();

    //    // Start path at random position on the left edge
    //    Vector2Int currentPosition = new Vector2Int(0, Random.Range(0, gridSizeZ));
    //    pathPositions.Add(currentPosition);


    //    while (currentPosition.x < gridSizeX - 1) // Until we reach the other side
    //    {
    //        List<Vector2Int> nextSteps = GetValidSteps(currentPosition);
    //        if (nextSteps.Count == 0)
    //        {
    //            Debug.LogError("No valid steps available. Path generation failed.");
    //            break;
    //        }

    //        currentPosition = nextSteps[Random.Range(0, nextSteps.Count)];
    //        pathPositions.Add(currentPosition);
    //    }

    //    // Add adjacent cells to towerPlacementZones
    //    foreach (Vector2Int pos in pathPositions)
    //    {
    //        AddAdjacentCells(pos);
    //    }

    ////    Debug.Log($"Path generated with {pathPositions.Count} positions.");
    ////    Debug.Log($"Tower placement zones: {towerPlacementZones.Count}");
    //
    //}



    private Vector2Int currentDirection = Vector2Int.zero;
    private int currentStraightLength = 0;

    void GeneratePath()
    {
        pathPositions = new List<Vector2Int>();
        towerPlacementZones = new List<Vector2Int>();

        // Start path at random position on the right edge, avoiding edges
        Vector2Int currentPosition = new Vector2Int(gridSizeX - 1, Random.Range(edgeAvoidance, gridSizeZ - edgeAvoidance));
        pathPositions.Add(currentPosition);

        Vector2Int endPosition = new Vector2Int(0, Random.Range(edgeAvoidance, gridSizeZ - edgeAvoidance)); // End on the left edge

        int maxSteps = (int)(gridSizeX * gridSizeZ * (2 - pathLengthFactor));
        int stepsTaken = 0;

        while (currentPosition != endPosition && stepsTaken < maxSteps)
        {
            List<Vector2Int> potentialSteps = new List<Vector2Int>();

            // If we've moved in the same direction for less than minStraightLength, keep going that way
            if (currentStraightLength < minStraightLength)
            {
                Vector2Int forcedStep = currentPosition + currentDirection;
                if (IsValidCell(forcedStep) && !pathPositions.Contains(forcedStep))
                {
                    potentialSteps.Add(forcedStep);
                }
            }
            else
            {
                // Possible moves: prioritize horizontal movement towards the end, then consider vertical
                int directionX = currentPosition.x > endPosition.x ? -1 : 1;

                Vector2Int horizontalStep = currentPosition + new Vector2Int(directionX, 0);
                if (IsValidCell(horizontalStep) && !pathPositions.Contains(horizontalStep))
                {
                    potentialSteps.Add(horizontalStep);
                }

                // Add vertical steps with a bias based on pathLengthFactor and a resistance to change
                if (Random.value < (1 - pathLengthFactor) && currentStraightLength < maxStraightLength)
                {
                    // Try to continue in the current vertical direction if possible
                    Vector2Int verticalStep = currentPosition + currentDirection;
                    if (IsValidCell(verticalStep) && !pathPositions.Contains(verticalStep))
                    {
                        potentialSteps.Add(verticalStep);
                    }
                    else
                    {
                        // If we can't continue, randomly choose a new vertical direction
                        int directionY = Random.value < 0.5 ? -1 : 1;
                        currentDirection = new Vector2Int(0, directionY);
                        verticalStep = currentPosition + currentDirection;
                        if (IsValidCell(verticalStep) && !pathPositions.Contains(verticalStep))
                        {
                            potentialSteps.Add(verticalStep);
                        }
                    }
                }
            }

            // If no valid steps, force a move towards the end
            if (potentialSteps.Count == 0)
            {
                int directionX = currentPosition.x > endPosition.x ? -1 : 1;
                Vector2Int horizontalStep = currentPosition + new Vector2Int(directionX, 0);

                // If a horizontal step is not possible, attempt a vertical step towards the end position's Y value
                if (!IsValidCell(horizontalStep) || pathPositions.Contains(horizontalStep))
                {
                    int directionY = endPosition.y - currentPosition.y;
                    directionY = directionY > 0 ? 1 : -1;
                    Vector2Int verticalStep = currentPosition + new Vector2Int(0, directionY);

                    if (IsValidCell(verticalStep) && !pathPositions.Contains(verticalStep))
                    {
                        potentialSteps.Add(verticalStep);
                    }
                }
                else
                {
                    potentialSteps.Add(horizontalStep);
                }
            }

            // Choose a random step from the potential steps
            if (potentialSteps.Count > 0)
            {
                Vector2Int nextStep = potentialSteps[Random.Range(0, potentialSteps.Count)];

                // Update current direction and straight length
                if (nextStep - currentPosition != currentDirection)
                {
                    currentDirection = nextStep - currentPosition;
                    currentStraightLength = 1;
                }
                else
                {
                    currentStraightLength++;
                }

                // Avoid getting too close to the edge
                if (nextStep.y < edgeAvoidance || nextStep.y >= gridSizeZ - edgeAvoidance)
                {
                    // Force a change in direction away from the edge
                    currentDirection = new Vector2Int(0, -currentDirection.y); // Reverse vertical direction
                    currentStraightLength = 1; // Reset straight length
                }

                currentPosition = nextStep;
                pathPositions.Add(currentPosition);
                stepsTaken++;
            }
            else
            {
                // If still no valid steps, break the loop to avoid infinite loop (should be extremely rare)
                break;
            }

            // Enforce the "only two neighbors" rule
            if (pathPositions.Count >= 3)
            {
                Vector2Int previousPosition = pathPositions[pathPositions.Count - 2];
                Vector2Int secondPreviousPosition = pathPositions[pathPositions.Count - 3];

                if (AreAdjacent(currentPosition, secondPreviousPosition))
                {
                    pathPositions.Remove(previousPosition);
                }
            }
        }

        // Add adjacent cells to towerPlacementZones
        foreach (Vector2Int pos in pathPositions)
        {
            AddAdjacentCells(pos);
        }

        Debug.Log($"Path generated with {pathPositions.Count} positions.");
        Debug.Log($"Tower placement zones: {towerPlacementZones.Count}");
    }

    bool AreAdjacent(Vector2Int cell1, Vector2Int cell2)
    {
        return Mathf.Abs(cell1.x - cell2.x) + Mathf.Abs(cell1.y - cell2.y) == 1;
    }




    //##############################/##########################

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

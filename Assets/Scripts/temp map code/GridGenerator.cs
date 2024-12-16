using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using System;
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



    // Variables for path generation
    private Vector2Int currentPosition;
    private int horizontalStraightCount;
    private int verticalStraightCount;
    private Vector2Int lastDirection;
    private bool justTurned;
    private int minStraightTilesAfterTurn;

    public float pathLengthFactor = 1.5f; // Factor to influence path length (closer to 1 means longer)
    public int maxHorizontalStraightTiles = 3; // Maximum horizontal tiles in a straight line
    public int maxVerticalStraightTiles = 15; // Maximum vertical tiles in a straight line
    public int minHorizontalStraightTiles = 2; // Minimum horizontal tiles in a straight line after a turn
    public int minVerticalStraightTiles = 10; // Minimum vertical tiles in a straight line after a turn

    void GeneratePath()
    {
        InitializePathGeneration();

        while (currentPosition.x > 0)
        {
            List<Vector2Int> nextSteps = GetNextSteps(currentPosition);

            if (nextSteps.Count == 0)
            {
                ForceTurn();
                nextSteps = GetNextSteps(currentPosition);
            }

            if (nextSteps.Count > 0)
            {
                TakeStep(nextSteps);
            }
            else
            {
                Debug.LogError("No valid steps available. Path generation failed.");
                break;
            }
        }

        AddAdjacentCellsToTowerZones();
        LogPathGenerationResults();
    }

    void InitializePathGeneration()
    {
        pathPositions = new List<Vector2Int>();
        towerPlacementZones = new List<Vector2Int>();

        currentPosition = new Vector2Int(gridSizeX - 1, UnityEngine.Random.Range(0, gridSizeZ));
        pathPositions.Add(currentPosition);

        horizontalStraightCount = 0;
        verticalStraightCount = 0;
        lastDirection = Vector2Int.zero;
        justTurned = false;
        minStraightTilesAfterTurn = 0;
    }

    List<Vector2Int> GetNextSteps(Vector2Int currentPosition)
    {
        List<Vector2Int> nextSteps = new List<Vector2Int>();
        Vector2Int[] possibleDirections = new Vector2Int[]
        {
        new Vector2Int(-1, 0),  // Move left (always allowed)
        new Vector2Int(0, 1),  // Move up
        new Vector2Int(0, -1)  // Move down
        };

        foreach (Vector2Int direction in possibleDirections)
        {
            int stepsToEdge = CalculateStepsToEdge(currentPosition, direction);

            // Adjust min/max straight tiles for this direction only
            int adjustedMaxStraightTiles = direction.x != 0 ? maxHorizontalStraightTiles : maxVerticalStraightTiles;
            int adjustedMinStraightTiles = direction.x != 0 ? minHorizontalStraightTiles : minVerticalStraightTiles;

            adjustedMaxStraightTiles = Math.Min(adjustedMaxStraightTiles, stepsToEdge);
            adjustedMinStraightTiles = Math.Min(adjustedMinStraightTiles, stepsToEdge);

            if (IsDirectionValid(direction, currentPosition, adjustedMinStraightTiles, adjustedMaxStraightTiles))
            {
                Vector2Int newPos = currentPosition + direction;
                if (IsValidStep(newPos))
                {
                    nextSteps.Add(newPos);
                }
            }
        }

        return nextSteps;
    }

    int CalculateStepsToEdge(Vector2Int position, Vector2Int direction)
    {
        if (direction.x != 0)
        { // Horizontal movement (towards the left edge)
            return position.x;
        }
        else if (direction.y > 0)
        { // Vertical movement (upwards)
            return gridSizeZ - 1 - position.y;
        }
        else
        { // Vertical movement (downwards)
            return position.y;
        }
    }

    bool IsDirectionValid(Vector2Int direction, Vector2Int currentPosition, int adjustedMinStraightTiles, int adjustedMaxStraightTiles)
    {
        // Check if the direction exceeds the adjusted maximum allowed straight tiles
        if (direction.x != 0 && horizontalStraightCount >= adjustedMaxStraightTiles) return false;
        if (direction.y != 0 && verticalStraightCount >= adjustedMaxStraightTiles) return false;

        // Ensure that after a turn, the path continues in the same direction for at least the adjusted minimum required straight tiles
        if (justTurned)
        {
            if (direction == lastDirection)
            {
                if (minStraightTilesAfterTurn < adjustedMinStraightTiles)
                {
                    return true; // Must continue in this direction
                }
            }
            else
            {
                return false; // Skip other directions if we haven't fulfilled the minimum straight tiles after a turn
            }
        }

        return true;
    }

    bool IsValidStep(Vector2Int newPos)
    {
        if (!IsValidCell(newPos) || pathPositions.Contains(newPos)) return false;

        int adjacentPathTiles = 0;
        Vector2Int[] possibleDirections = new Vector2Int[]
        {
        new Vector2Int(-1, 0),  // Move left (always allowed)
        new Vector2Int(0, 1),  // Move up
        new Vector2Int(0, -1)  // Move down
        };

        foreach (Vector2Int neighborDirection in possibleDirections)
        {
            Vector2Int neighborPos = newPos + neighborDirection;
            if (pathPositions.Contains(neighborPos))
            {
                adjacentPathTiles++;
            }
        }

        return adjacentPathTiles < 2;
    }

    void ForceTurn()
    {
        horizontalStraightCount = 0;
        verticalStraightCount = 0;
        justTurned = true; // We're about to force a turn
    }

    void TakeStep(List<Vector2Int> nextSteps)
    {
        Vector2Int chosenStep = nextSteps[UnityEngine.Random.Range(0, nextSteps.Count)];

        // Calculate direction before updating currentPosition
        Vector2Int direction = chosenStep - currentPosition;

        currentPosition = chosenStep;
        pathPositions.Add(currentPosition);

        UpdateStraightTileCounts(direction);
        UpdateTurnFlags(direction);
    }

    void UpdateStraightTileCounts(Vector2Int direction)
    {
        if (direction.x != 0)
        { // Moved left
            horizontalStraightCount++;
            verticalStraightCount = 0;

            if (justTurned)
            {
                minStraightTilesAfterTurn++;
            }
        }
        else if (direction.y != 0)
        { // Moved up or down
            verticalStraightCount++;
            horizontalStraightCount = 0;

            if (justTurned)
            {
                minStraightTilesAfterTurn++;
            }
        }
    }

    void UpdateTurnFlags(Vector2Int direction)
    {
        if (lastDirection != Vector2Int.zero && direction != lastDirection)
        {
            justTurned = true; // We made a turn
            minStraightTilesAfterTurn = 0; // Reset the counter after a turn
        }
        else if (!justTurned)
        {
            justTurned = false;
        }

        if (justTurned && minStraightTilesAfterTurn >= (lastDirection.x != 0 ? minHorizontalStraightTiles : minVerticalStraightTiles))
        {
            justTurned = false;
            minStraightTilesAfterTurn = 0;
        }

        lastDirection = direction;
    }

    void AddAdjacentCellsToTowerZones()
    {
        foreach (Vector2Int pos in pathPositions)
        {
            AddAdjacentCells(pos);
        }
    }

    void LogPathGenerationResults()
    {
        Debug.Log($"Path generated with {pathPositions.Count} positions.");
        Debug.Log($"Tower placement zones: {towerPlacementZones.Count}");
    }













    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

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
            if (UnityEngine.Random.Range(0, 4) == 0)
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

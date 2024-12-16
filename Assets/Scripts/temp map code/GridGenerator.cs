using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using System;
public class GridGenerator : MonoBehaviour
{
    public GameObject cellPrefab; // Assign a cube or custom cell prefab in the Inspector
    public GameObject towerPrefab;
    public GameObject grassPrefab; // Grass decoration
    public GameObject rockPrefab; // Rock decoration
    public int gridSizeX = 16;
    public int gridSizeZ = 16;
    public float cellSize = 1f;


    public List<Vector2Int> shortestPath { get; private set; } // Store the shortest path

    //private List<Vector2Int> pathPositions;
    public List<Vector2Int> pathPositions { get; private set; } // Expose the path positions

    public List<Vector2Int> towerPositions { get; private set; } // Expose the path positions
    private Dictionary<Vector2Int, GameObject> gridCells = new Dictionary<Vector2Int, GameObject>(); // To track cells for coloring
    public List<Vector2Int> towerPlacementZones { get; private set; }


    void Start()
    {
        GenerateGrid();
        GeneratePath();
        //DecorateGrid();

    }






    public float pathLengthFactor = 1.5f;
    public int maxHorizontalStraightTiles = 3;
    public int maxVerticalStraightTiles = 15;
    public int minHorizontalStraightTiles = 2;
    public int minVerticalStraightTiles = 10;
    public float pathBranchingProbability = 0.2f; // Probability of branching at any given step
    public int minBranchLength = 5; // Minimum length of a branch before it can merge
    public int maxBranches = 3; // Maximum number of simultaneous branches

    // Path generation variables (now declared as private member variables)
    private Vector2Int currentPosition;
    private int horizontalStraightCount;
    private int verticalStraightCount;
    private Vector2Int lastDirection;
    private bool justTurned;
    private int minStraightTilesAfterTurn;


    private class PathBranch
    {
        public Vector2Int position;
        public int length;
        public int horizontalStraightCount;
        public int verticalStraightCount;
        public Vector2Int lastDirection;
        public bool justTurned;
        public int minStraightTilesAfterTurn;
        public HashSet<Vector2Int> branchPathPositions; // Store positions for each branch (HashSet for efficiency)

        public PathBranch(Vector2Int startPosition)
        {
            position = startPosition;
            length = 0;
            horizontalStraightCount = 0;
            verticalStraightCount = 0;
            lastDirection = Vector2Int.zero;
            justTurned = false;
            minStraightTilesAfterTurn = 0;
            branchPathPositions = new HashSet<Vector2Int> { startPosition };
        }
    }

    private List<PathBranch> activeBranches = new List<PathBranch>();

    void GeneratePath()
    {
        // Initialize path generation (all variables are now local)
        pathPositions = new List<Vector2Int>();
        towerPlacementZones = new List<Vector2Int>();
        activeBranches.Clear();

        Vector2Int startPosition = new Vector2Int(gridSizeX - 1, UnityEngine.Random.Range(0, gridSizeZ));
        activeBranches.Add(new PathBranch(startPosition));

        while (activeBranches.Any(branch => branch.position.x > 0))
        {
            for (int i = activeBranches.Count - 1; i >= 0; i--)
            {
                PathBranch branch = activeBranches[i];
                if (branch.position.x <= 0) continue;

                List<Vector2Int> nextSteps = GetNextSteps(branch);

                if (nextSteps.Count == 0)
                {
                    if (!HandleNoValidSteps(branch))
                    {
                        Debug.LogError($"Path branch generation failed at {branch.position}");
                        activeBranches.Remove(branch);
                    }
                }
                else
                {
                    // Attempt branching while choosing a step
                    if (activeBranches.Count < maxBranches && UnityEngine.Random.value < pathBranchingProbability)
                    {
                        PathBranch newBranch = new PathBranch(branch.position);
                        activeBranches.Add(newBranch);
                        Debug.Log($"New branch created at {newBranch.position}");
                    }

                    TakeStep(nextSteps, branch); // TakeStep now handles merges internally
                    AttemptMerging(branch); // Merging logic updated
                }
            }
        }

        CombineBranchPositions(); // Using HashSet during generation makes this more efficient
        AddAdjacentCellsToTowerZones();
        LogPathGenerationResults();
    }



    private void InitializePathGeneration()
    {
        pathPositions = new List<Vector2Int>();
        towerPlacementZones = new List<Vector2Int>();

        currentPosition = new Vector2Int(gridSizeX - 1, UnityEngine.Random.Range(0, gridSizeZ));
        //pathPositions.Add(currentPosition); // Don't add initial position here

        horizontalStraightCount = 0;
        verticalStraightCount = 0;
        lastDirection = Vector2Int.zero;
        justTurned = false;
        minStraightTilesAfterTurn = 0;

        activeBranches.Clear();
    }

    List<Vector2Int> GetNextSteps(PathBranch branch)
    {
        List<Vector2Int> nextSteps = new List<Vector2Int>();
        Vector2Int[] possibleDirections = new Vector2Int[]
        {
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };

        foreach (Vector2Int direction in possibleDirections)
        {
            if (IsDirectionValid(direction, branch))
            {
                Vector2Int newPos = branch.position + direction;
                if (IsValidStep(newPos, branch))
                {
                    nextSteps.Add(newPos);
                }
            }
        }

        return nextSteps;
    }

    bool IsDirectionValid(Vector2Int direction, PathBranch branch)
    {
        // Check for maximum straight tiles
        if (direction.x != 0 && branch.horizontalStraightCount >= maxHorizontalStraightTiles) return false;
        if (direction.y != 0 && branch.verticalStraightCount >= maxVerticalStraightTiles) return false;

        // Minimum straight tiles after a turn
        if (branch.justTurned)
        {
            if (direction != branch.lastDirection) return false;
            if (branch.minStraightTilesAfterTurn < (branch.lastDirection.x != 0 ? minHorizontalStraightTiles : minVerticalStraightTiles))
            {
                return true;
            }
        }

        return true;
    }

    bool IsValidStep(Vector2Int newPos, PathBranch branch)
    {
        if (!IsValidCell(newPos)) return false;

        // Prevent merging with itself
        if (branch.branchPathPositions.Contains(newPos)) return false;

        // Allow merging with other branches
        if (activeBranches.Any(b => b != branch && b.branchPathPositions.Contains(newPos))) return true;

        // Check for adjacent path tiles from any branch
        int adjacentPathTiles = 0;
        foreach (Vector2Int neighborDirection in new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) })
        {
            if (activeBranches.Any(b => b.branchPathPositions.Contains(newPos + neighborDirection)))
            {
                adjacentPathTiles++;
            }
        }

        return adjacentPathTiles < 2;
    }

    void TakeStep(List<Vector2Int> nextSteps, PathBranch branch)
    {
        // Prioritize the left direction if available and close to the end
        Vector2Int chosenStep;
        if (branch.position.x == 1 && nextSteps.Contains(branch.position + Vector2Int.left))
        {
            chosenStep = branch.position + Vector2Int.left;
        }
        else
        {
            chosenStep = nextSteps[UnityEngine.Random.Range(0, nextSteps.Count)];
        }

        // Calculate direction before updating branch position
        Vector2Int direction = chosenStep - branch.position;

        // Check if this step merges with another branch
        var mergingBranch = activeBranches.FirstOrDefault(b => b != branch && b.branchPathPositions.Contains(chosenStep));
        if (mergingBranch != null)
        {
            // Merge branches (updated logic)
            MergeBranches(branch, mergingBranch);
        }
        else
        {
            // Normal step
            branch.position = chosenStep;
            branch.branchPathPositions.Add(chosenStep); // Using HashSet, no need to check for duplicates
            branch.length++;

            UpdateStraightTileCounts(direction, branch);
            UpdateTurnFlags(direction, branch);
        }
    }



    bool HandleNoValidSteps(PathBranch branch)
    {
        // Check if we are at the leftmost position
        if (branch.position.x == 0)
        {
            return true; // Path generation successful for this branch
        }

        // Try forcing a left move if possible
        if (branch.position.x > 0 && IsValidStep(branch.position + Vector2Int.left, branch))
        {
            TakeStep(new List<Vector2Int> { branch.position + Vector2Int.left }, branch);
            return true;
        }

        // If no valid steps, attempt to backtrack for the branch
        return Backtrack(branch);
    }

    bool Backtrack(PathBranch branch)
    {
        if (branch.branchPathPositions.Count <= 1)
        {
            Debug.Log($"Backtrack failed for branch at {branch.position} - no positions left to backtrack.");
            return false; // Cannot backtrack further
        }

        // Store the last position for debugging
        Vector2Int lastPosition = branch.position;

        // Remove the last position from the branch
        branch.branchPathPositions.Remove(branch.branchPathPositions.Last());

        // Check if the branch is now empty after removing the last position
        if (branch.branchPathPositions.Count == 0)
        {
            Debug.Log($"Branch at {lastPosition} became empty after backtracking.");
            return false; // Branch is empty
        }

        // Update the current position of the branch to the new last position
        branch.position = branch.branchPathPositions.Last();

        // Reset turn flags and straight tile counts
        branch.justTurned = false;
        branch.minStraightTilesAfterTurn = 0;
        if (branch.lastDirection.x != 0) branch.horizontalStraightCount = 0;
        if (branch.lastDirection.y != 0) branch.verticalStraightCount = 0;

        // Recalculate last direction for the branch
        if (branch.branchPathPositions.Count > 1)
        {
            branch.lastDirection = branch.position - branch.branchPathPositions.ToList()[branch.branchPathPositions.Count - 2];
        }
        else
        {
            branch.lastDirection = Vector2Int.zero;
        }

        Debug.Log($"Backtracked from {lastPosition} to {branch.position}");
        return true; // Successfully backtracked
    }

    void UpdateStraightTileCounts(Vector2Int direction, PathBranch branch)
    {
        if (direction.x != 0)
        {
            branch.horizontalStraightCount++;
            branch.verticalStraightCount = 0;
        }
        else
        {
            branch.verticalStraightCount++;
            branch.horizontalStraightCount = 0;
        }

        if (branch.justTurned)
        {
            branch.minStraightTilesAfterTurn++;
        }
    }

    void UpdateTurnFlags(Vector2Int direction, PathBranch branch)
    {
        if (direction != branch.lastDirection)
        {
            branch.justTurned = true;
            branch.minStraightTilesAfterTurn = 0;
        }
        else if (branch.justTurned && branch.minStraightTilesAfterTurn >= (direction.x != 0 ? minHorizontalStraightTiles : minVerticalStraightTiles))
        {
            branch.justTurned = false;
        }

        branch.lastDirection = direction;
    }

    void AttemptBranching(PathBranch branch)
    {
        if (activeBranches.Count >= maxBranches) return;

        if (UnityEngine.Random.value < pathBranchingProbability)
        {
            PathBranch newBranch = new PathBranch(branch.position);
            activeBranches.Add(newBranch);
            Debug.Log($"New branch created at {newBranch.position}");
        }
    }

    void AttemptMerging(PathBranch branch)
    {
        if (branch.length < minBranchLength) return;

        for (int i = activeBranches.Count - 1; i >= 0; i--)
        {
            PathBranch otherBranch = activeBranches[i];
            if (otherBranch == branch || otherBranch.length < minBranchLength) continue;

            // Check if any position in otherBranch is adjacent to the current branch's position
            if (otherBranch.branchPathPositions.Any(pos => IsAdjacent(branch.position, pos)))
            {
                MergeBranches(branch, otherBranch);
                Debug.Log($"Merged branches at {branch.position}");
                return; // Exit after merging
            }
        }
    }

    bool IsAdjacent(Vector2Int pos1, Vector2Int pos2)
    {
        return Mathf.Abs(pos1.x - pos2.x) <= 1 && Mathf.Abs(pos1.y - pos2.y) <= 1;
    }


    bool AreBranchesClose(PathBranch branch1, PathBranch branch2)
    {
        // Check if branches are within one cell of each other
        return Mathf.Abs(branch1.position.x - branch2.position.x) <= 1 &&
               Mathf.Abs(branch1.position.y - branch2.position.y) <= 1;
    }

    void MergeBranches(PathBranch branch1, PathBranch branch2)
    {
        // Merge the positions of the shorter branch into the longer one
        PathBranch longerBranch = branch1.length > branch2.length ? branch1 : branch2;
        PathBranch shorterBranch = branch1.length > branch2.length ? branch2 : branch1;

        // Add positions from shorter branch to the longer one (HashSet handles uniqueness)
        longerBranch.branchPathPositions.UnionWith(shorterBranch.branchPathPositions);

        // Update the length of the longer branch
        longerBranch.length = longerBranch.branchPathPositions.Count;

        // Remove the shorter branch
        activeBranches.Remove(shorterBranch);
    }

    void CombineBranchPositions()
    {
        // Combine all branch positions into the main pathPositions list (more efficient with HashSet)
        pathPositions.Clear();
        foreach (var branch in activeBranches)
        {
            pathPositions.AddRange(branch.branchPathPositions); // Still need AddRange for the final list
        }
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



    void CalculateShortestPath()
    {
        shortestPath = Dijkstra.FindShortestPath(pathPositions, new Vector2Int(gridSizeX - 1, pathPositions[0].y), Vector2Int.zero);
    }
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
                cell.tag = "grass";


                cell.GetComponent<Renderer>().material.color = Color.green; // Default tile color





                Instantiate(grassPrefab, cell.transform.position + Vector3.up * 0.5f, Quaternion.identity, cell.transform);

                //Instantiate(rockPrefab, cell.transform.position + Vector3.up * 1f, Quaternion.identity, cell.transform);


                // Track cells in a dictionary for easy recoloring
                gridCells[new Vector2Int(x, z)] = cell;

                // Randomly decide to place grass or rocks
                float randomChance = UnityEngine.Random.value;
                // only place rocks on grass tiles
                if (randomChance < 0.05f)
                {
                    Instantiate(rockPrefab, cell.transform.position + Vector3.up * 0.5f, Quaternion.identity, cell.transform);
                }

            }
        }
    }



    public void InitializeGrid()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject); // Clear previous cells
        }

        gridCells.Clear();
        GenerateGrid();
        GeneratePath();
        CalculateShortestPath(); // Recalculate if grid is reinitialized
        foreach (Vector2Int pos in pathPositions)
        {
            if (gridCells.TryGetValue(pos, out GameObject cell))
            {
                //cell.GetComponent<Renderer>().material.color = Color.blue; // Blue for the path
                //cell.GetComponent<Renderer>().material.color = new Color(0.5f, 0.35f, 0.05f);
                cell.GetComponent<Renderer>().material.color = new Color(0.4f, 0.25f, 0.05f);

                foreach (Transform child in cell.transform)
                {
                    Destroy(child.gameObject); // Destroy all child objects (grass, rocks, etc.)
                }

            }
        }

        // Process tower placement zones
        foreach (Vector2Int pos in towerPlacementZones)
        {
            if (gridCells.TryGetValue(pos, out GameObject cell))
            {
                // Change the color of the tower placement tiles
                cell.GetComponent<Renderer>().material.color = new Color(0.0f, 0.5f, 0.0f); // Green for tower zones

                // Remove grass or rocks from tower placement zones
                foreach (Transform child in cell.transform)
                {
                    Destroy(child.gameObject); // Destroy all child objects (grass, rocks, etc.)
                }
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

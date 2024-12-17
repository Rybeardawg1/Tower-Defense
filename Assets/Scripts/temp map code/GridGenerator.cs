using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using System;
public class GridGenerator : MonoBehaviour
{
    public GameObject cellPrefab;
    public GameObject towerPrefab;
    public GameObject grassPrefab; 
    public GameObject rockPrefab; 
    public int gridSizeX = 16;
    public int gridSizeZ = 16;
    public float cellSize = 1f;


    public List<Vector2Int> shortestPath { get; private set; } // Store shortest path
    public List<Vector2Int> pathPositions { get; private set; } // Positions
    public List<Vector2Int> towerPositions { get; private set; }
    private Dictionary<Vector2Int, GameObject> gridCells = new Dictionary<Vector2Int, GameObject>(); // To track cells for coloring
    public List<Vector2Int> towerPlacementZones { get; private set; }


    void Start()
    {
        GenerateGrid();
        GeneratePath();

    }






    public float pathLengthFactor = 1.5f;
    public int maxHorizontalStraightTiles = 3;
    public int maxVerticalStraightTiles = 15;
    public int minHorizontalStraightTiles = 2;
    public int minVerticalStraightTiles = 10;
    public float pathBranchingProbability = 0.2f; 
    public int minBranchLength = 5; 
    public int maxBranches = 3; 


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
        public HashSet<Vector2Int> branchPathPositions;

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
                    if (activeBranches.Count < maxBranches && UnityEngine.Random.value < pathBranchingProbability)
                    {
                        PathBranch newBranch = new PathBranch(branch.position);
                        activeBranches.Add(newBranch);
                        Debug.Log($"New branch created at {newBranch.position}");
                    }

                    TakeStep(nextSteps, branch); // TakeStep now handles merges internally
                    AttemptMerging(branch); 
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
        // Prioritize the left direction and close to the end
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
            // Merge branches 
            MergeBranches(branch, mergingBranch);
        }
        else
        {
            // Normal step
            branch.position = chosenStep;
            branch.branchPathPositions.Add(chosenStep); // No need to check for duplicates
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
            //Debug.Log($"Backtrack failed for branch at {branch.position}");
            return false; // Cannot backtrack further
        }

        // Store for debugging
        Vector2Int lastPosition = branch.position;

        // Remove from the branch
        branch.branchPathPositions.Remove(branch.branchPathPositions.Last());

        // Check if the branch is now empty
        if (branch.branchPathPositions.Count == 0)
        {
            //Debug.Log($"Branch at {lastPosition} became empty after backtracking.");
            return false; // Branch is empty
        }

        // Update the current position of the branch
        branch.position = branch.branchPathPositions.Last();

        // Reset counts
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

        //Debug.Log($"Backtracked from {lastPosition} to {branch.position}");
        return true; // Successful backtrack
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
            //Debug.Log($"New branch created at {newBranch.position}");
        }
    }

    void AttemptMerging(PathBranch branch)
    {
        if (branch.length < minBranchLength) return;

        for (int i = activeBranches.Count - 1; i >= 0; i--)
        {
            PathBranch otherBranch = activeBranches[i];
            if (otherBranch == branch || otherBranch.length < minBranchLength) continue;
            if (otherBranch.branchPathPositions.Any(pos => IsAdjacent(branch.position, pos)))
            {
                MergeBranches(branch, otherBranch);
                //Debug.Log($"Merged branches at {branch.position}");
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
        // Merge the positions of the shorter branch
        PathBranch longerBranch = branch1.length > branch2.length ? branch1 : branch2;
        PathBranch shorterBranch = branch1.length > branch2.length ? branch2 : branch1;
        longerBranch.branchPathPositions.UnionWith(shorterBranch.branchPathPositions);
        longerBranch.length = longerBranch.branchPathPositions.Count;
        activeBranches.Remove(shorterBranch);
    }

    void CombineBranchPositions()
    {
        // Combine all branch positions into the main pathPositions list
        pathPositions.Clear();
        foreach (var branch in activeBranches)
        {
            pathPositions.AddRange(branch.branchPathPositions);
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



    public void CalculateShortestPath()
    {
        shortestPath = Dijkstra.FindShortestPath(pathPositions, new Vector2Int(gridSizeX - 1, pathPositions[0].y), Vector2Int.zero);
    }

    //public List<Vector2Int> CalculateShortestPath_cont(Vector2Int startPosition)
    //{
    //    // Use the provided startPosition instead of a fixed one
    //    shortestPath = Dijkstra.FindShortestPath(pathPositions, startPosition, Vector2Int.zero);
    //    return shortestPath;
    //}
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
                // cell.tag = "green cell";


                cell.GetComponent<Renderer>().material.color = Color.green; // Default tile color





                Instantiate(grassPrefab, cell.transform.position + Vector3.up * 0.5f, Quaternion.identity, cell.transform);

                //Instantiate(rockPrefab, cell.transform.position + Vector3.up * 1f, Quaternion.identity, cell.transform);


                // Track cells in a dictionary
                gridCells[new Vector2Int(x, z)] = cell;
                float randomChance = UnityEngine.Random.value;
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
            Destroy(child.gameObject); 
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
                cell.GetComponent<Renderer>().material.color = new Color(0.4f, 0.25f, 0.05f);

                foreach (Transform child in cell.transform)
                {
                    Destroy(child.gameObject); 
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
                    Destroy(child.gameObject); 
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

                Debug.LogWarning($"No Renderer found on cell {cell.name}");
            }
        }
    }
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

            // Add sparsity
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

        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 0), 
            new Vector2Int(0, 1), 
            new Vector2Int(0, -1),
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
                if (!pathPositions.Contains(newPos)) //so we dont go through the path
                {
                    steps.Add(newPos);
                }
            }
        }

        return steps;
    }
}

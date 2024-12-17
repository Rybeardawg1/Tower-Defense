using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor;


//using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Jobs;

public class EnemyManager : MonoBehaviour
{
    private static Queue<Enemy> enemies_to_kill_queue;
    private static Queue<int> enemies_to_spawn;
    private bool Endloop = false;

    public static Vector3[] node_grid; // this is the path that the enemies will follow
    public GridGenerator gridGenerator;

    [Header("Enemy Spawn Settings")]
    public int nextEnemyID = 1;

    void Awake()
    {
        if (gridGenerator == null)
        {
            gridGenerator = FindObjectOfType<GridGenerator>();
            if (gridGenerator == null)
            {
                Debug.LogError("GridGenerator not found! Please make sure it's attached in the scene.");
            }
        }
    }

    void Start()
    {

        gridGenerator.InitializeGrid();

        List<Vector2Int> pathPositions = gridGenerator.shortestPath; // Use shortestPath to get the best path
        if (pathPositions == null || pathPositions.Count == 0)
        {
            Debug.LogError("Path generation failed. Make sure GridGenerator is set up properly.");
            return;
        }
        Debug.Log($"Path created with {pathPositions.Count} steps.");


        enemies_to_spawn = new Queue<int>();
        enemies_to_kill_queue = new Queue<Enemy>();
        Summoner.Init();

        // Convert path positions to world positions for enemies (using shortest path)
        node_grid = new Vector3[pathPositions.Count];
        for (int i = 0; i < pathPositions.Count; i++)
        {
            Vector2Int gridPos = pathPositions[i];
            node_grid[i] = new Vector3(gridPos.x * gridGenerator.cellSize, 0, gridPos.y * gridGenerator.cellSize);
        }


        // start the enemy loop
        StartCoroutine(EnemyLoop());

        // Test summoning an enemy repeatedly
        // make call time random
        // i want a random time between 0.7 and 1 seconds so we need a coroutine
        StartCoroutine(SummonEnemyLoop());
    }




    public void PlaceTower(Vector2Int position)
    {
        if (gridGenerator.towerPlacementZones.Contains(position))
        {
            Debug.Log($"Placing tower at {position}");
        }
        else
        {
            Debug.LogError("Invalid position for tower placement!");
        }
    }

    IEnumerator SummonEnemyLoop()
    {
        while (true)
        {
            //// Stop summoning if nextEnemyID is 0
            if (nextEnemyID == 0)
            {
                Debug.Log("Stopped summoning enemies as ID is 0.");
                // loop without summoning
                yield return new WaitForSeconds(1);
            }

            enqueue_enemy_to_spawn(nextEnemyID);
            float waitTime = GetRandomSummonTime();

            //float waitTime = GetRandomSummonTime(); // Get a new random time
            yield return new WaitForSeconds(3f); // Wait for that time before the next summon
        }
    }
    float GetRandomSummonTime()
    {
        return Random.Range(0.7f, 3f); // not working
    }

    IEnumerator EnemyLoop()
    {
        while (Endloop == false)
        {
            SpawnEnemies();
            UpdateEnemies();
            RemoveEnemies();

            yield return null; // Wait for the next frame
        }
    }

    public static void enqueue_enemy_to_spawn(int enemy_ID)
    {

        enemies_to_spawn.Enqueue(enemy_ID);

    }

    void SpawnEnemies()
    {
        if (enemies_to_spawn.Count > 0)
        {
            for (int i = 0; i < enemies_to_spawn.Count; i++)
            {
                int enemy_ID = enemies_to_spawn.Dequeue();
                Summoner.spawn_enemy(enemy_ID);
            }
        }
    }

    void UpdateEnemies()
    {
        foreach (var enemy in Summoner.enemies_alive)
        {
            if (enemy.isAlive && enemy != null)
            {
                enemy.Perform_movement();
            }
        }

    }

    void RemoveEnemies()
    {
        if (enemies_to_kill_queue.Count > 0)
        {
            for (int i = 0; i < enemies_to_kill_queue.Count; i++)
            {
                Enemy enemy_to_kill = enemies_to_kill_queue.Dequeue();
                Summoner.remove_enemy(enemy_to_kill);
            }
        }
    }

    public static void enqueue_enemy_to_kill(Enemy enemy_to_kill)
    {
        enemies_to_kill_queue.Enqueue(enemy_to_kill);
    }
}
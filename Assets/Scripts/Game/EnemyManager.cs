using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Jobs;

public class EnemyManager : MonoBehaviour
{
    private static Queue<Enemy> enemies_to_kill_queue;
    private static Queue<int> enemies_to_spawn;
    private bool Endloop = false;
    private int current_wave = 1;
    private int enemies_this_wave = 0; // Track the number of enemies to spawn for this wave
    public Text waveText;

    public static Vector3[] node_grid; // this is the path that the enemies will follow
    public GridGenerator gridGenerator; // Reference to GridGenerator

    [Header("Enemy Spawn Settings")]
    public int nextEnemyID = 1; // Public field to dynamically set the next enemy ID

    void Start()
    {
        if (gridGenerator == null)
        {
            Debug.LogError("GridGenerator is not assigned.");
            return;
        }
        Debug.Log("Initializing grid and path...");
        gridGenerator.InitializeGrid();

        List<Vector2Int> pathPositions = gridGenerator.shortestPath; // Use shortestPath now
        if (pathPositions == null || pathPositions.Count == 0)
        {
            Debug.LogError("Path generation failed. Ensure GridGenerator is set up properly.");
            return;
        }

        Debug.Log($"Path fetched with {pathPositions.Count} steps.");

        enemies_to_spawn = new Queue<int>();
        enemies_to_kill_queue = new Queue<Enemy>();
        Summoner.Init();

        node_grid = new Vector3[pathPositions.Count];
        for (int i = 0; i < pathPositions.Count; i++)
        {
            Vector2Int gridPos = pathPositions[i];
            node_grid[i] = new Vector3(gridPos.x * gridGenerator.cellSize, 0, gridPos.y * gridGenerator.cellSize);
        }

        // Start the enemy loop coroutine
        StartCoroutine(EnemyLoop());

        // Start the summoning loop
        StartCoroutine(SummonEnemyLoop());
    }

    void Awake()
    {
        if (gridGenerator == null)
        {
            gridGenerator = FindObjectOfType<GridGenerator>();
            if (gridGenerator == null)
            {
                Debug.LogError("GridGenerator not found! Please ensure it's attached in the scene.");
            }
        }
    }

    void Update() {
        waveText.text = $"Wave: {current_wave}";
    }

    IEnumerator SummonEnemyLoop()
    {
        while (true) // Limit to 10 waves
        {   
            // Determine how many enemies to spawn this wave
            enemies_this_wave = 2 * (current_wave + 1); // 2 * wave number

            // Spawn enemies for the current wave
            for (int i = 0; i < enemies_this_wave; i++)
            {
                nextEnemyID = 1;
                enqueue_enemy_to_spawn(nextEnemyID);
                float wait_time = Random.Range(1.0f, 3.0f);
                yield return new WaitForSeconds(wait_time);
            }
            Debug.Log("num enemy: " + enemies_to_spawn.Count);

            // Wait for the enemies to be spawned before starting the next wave
            yield return new WaitForSeconds(8f);

            // Move to the next wave
            current_wave++;

            // If we've completed all 10 waves, stop spawning
            if (current_wave >= 10)
            {
                Debug.Log("All waves have been spawned.");
                yield break;
            }
        }
    }

    IEnumerator EnemyLoop()
    {
        while (!Endloop)
        {
            SpawnEnemies();
            UpdateEnemies();
            ApplyEffects();
            RemoveEnemies();
            yield return null; // Wait for the next frame
        }
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < enemies_to_spawn.Count; i++)
        {
            int enemy_ID = enemies_to_spawn.Dequeue();
            Summoner.spawn_enemy(enemy_ID);
        }
    }

    void UpdateEnemies()
    {
        foreach (var enemy in Summoner.enemies_alive)
        {
            if (enemy.isAlive)
            {
                enemy.Perform_movement(); // Call the new method
            }
        }
    }

    void ApplyEffects()
    {
        // Implement status effect applications here
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
        enemies_to_kill_queue.Enqueue(enemy_to_kill); // this will add the enemy to the queue of enemies to kill next
    }

    public static void enqueue_enemy_to_spawn(int enemy_ID)
    {
        enemies_to_spawn.Enqueue(enemy_ID); // this will add the enemy ID to the queue of enemies to spawn next
    }
}

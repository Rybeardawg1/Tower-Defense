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
    // public Transform node_parent; // this is the parent object that holds the nodes

    public GridGenerator gridGenerator; // Reference to GridGenerator
    ////
    [Header("Enemy Spawn Settings")]
    public int nextEnemyID = 1; // Public field to dynamically set the next enemy ID
    ////

    void Start()
    {
        if (gridGenerator == null)
        {
            Debug.LogError("GridGenerator is not assigned.");
            return;
        }
        Debug.Log("Initializing grid and path...");
        gridGenerator.InitializeGrid();

        // Fetch path after reinitialization
        List<Vector2Int> pathPositions = gridGenerator.pathPositions;
        if (pathPositions == null || pathPositions.Count == 0)
        {
            Debug.LogError("Path generation failed. Ensure GridGenerator is set up properly.");
            return;
        }

        Debug.Log($"Path fetched with {pathPositions.Count} steps.");



        enemies_to_spawn = new Queue<int>();
        enemies_to_kill_queue = new Queue<Enemy>();
        Summoner.Init();



        // Convert path positions to world positions for enemies
        node_grid = new Vector3[pathPositions.Count];
        //node_grid = new Vector3[node_parent.childCount]; // the size of the array is the number of children in the node_parent object

        //for (int i = 0; i < node_grid.Length; i++) 
        //{
        //    node_grid[i] = node_parent.GetChild(i).position; // get the position of the child and add it to the array
        //}
        for (int i = 0; i < pathPositions.Count; i++)
        {
            Vector2Int gridPos = pathPositions[i];
            node_grid[i] = new Vector3(gridPos.x * gridGenerator.cellSize, 0, gridPos.y * gridGenerator.cellSize);
        }


        // start the enemy loop coroutine
        StartCoroutine(EnemyLoop()); // this is the IEnumerator loop below that will run the game loop for the enemies

        // Test summoning an enemy repeatedly
        //InvokeRepeating("Summon_test", 0, 1); // this will call the Summon_test function every 1 second starting from 0 seconds
        // make call time random
        //InvokeRepeating("Summon_test", 0, 1);
        // i want a random time between 0.7 and 1 seconds so we need a coroutine
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

    public void PlaceTower(Vector2Int position)
    {
        if (gridGenerator.towerPlacementZones.Contains(position))
        {
            Debug.Log($"Placing tower at {position}");
            // Logic to place the tower, e.g., instantiate a prefab
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
            enqueue_enemy_to_spawn(nextEnemyID); // Add the next enemy ID to the queue
            float waitTime = GetRandomSummonTime();

            //Summon_test(); // Call the summon test function
            //float waitTime = GetRandomSummonTime(); // Get a new random time
            yield return new WaitForSeconds(3f); // Wait for that time before the next summon
        }
    }
    float GetRandomSummonTime()
    {
        return Random.Range(0.7f, 3f); // not working
    }
    //void remove_test()
    //{
    //    if (Summoner.enemies_alive.Count > 0)
    //    {
    //        Summoner.remove_enemy(Summoner.enemies_alive[Random.Range(0, Summoner.enemies_alive.Count)]);
    //    }
    //}
    //void Summon_test()
    //{
    //    enqueue_enemy_to_spawn(1); // 1 is the id we gave to the enemy in the Enemy_spawn_data scriptable object we created
    //}


    IEnumerator EnemyLoop()
    {
        while (Endloop == false)
        {
            SpawnEnemies();
            UpdateEnemies();
            ApplyEffects();
            RemoveEnemies();

            yield return null; // Wait for the next frame
        }
    }

    public static void enqueue_enemy_to_spawn(int enemy_ID)
    {

        enemies_to_spawn.Enqueue(enemy_ID); // this will add the enemy ID to the queue of enemies to spawn next time it  reaches the spawn enemy function in the IEnumerator loop

    }

    void SpawnEnemies()
    {
        // Implement enemy spawning
        if (enemies_to_spawn.Count > 0)
        {
            for (int i = 0; i < enemies_to_spawn.Count; i++)
            {
                int enemy_ID = enemies_to_spawn.Dequeue();
                Summoner.spawn_enemy(enemy_ID);
                Debug.Log($"Spawned enemy with ID {enemy_ID}");
            }
        }

    }

    void UpdateEnemies()
    {
        //Debug.Log($"Updating {Summoner.enemies_alive.Count} enemies.");

        // Implement enemy movement and AI updates here
        NativeArray<int> node_indexes = new NativeArray<int>(Summoner.enemies_alive.Count, Allocator.TempJob); // This is for storing the node index of each enemy
        NativeArray<float> enemy_speeds = new NativeArray<float>(Summoner.enemies_alive.Count, Allocator.TempJob); // This is for storing the speed of each enemy
        NativeArray<Vector3> to_use_nodes = new NativeArray<Vector3>(node_grid, Allocator.TempJob); // This is for storing the nodes that the enemies will follow (get from the generated map?)
        TransformAccessArray enemy_access = new TransformAccessArray(Summoner.enemies_alive_transform.ToArray()); // This is for storing the transforms of the enemies

        // create array just to recognize flying enemies
        NativeArray<bool> is_flying = new NativeArray<bool>(Summoner.enemies_alive.Count, Allocator.TempJob);


        //NativeArray<Animator> animators = new NativeArray<Animator>(Summoner.enemies_alive.Count, Allocator.TempJob);
        //for (int i = 0; i < Summoner.enemies_alive.Count; i++)
        //{
        //    animators[i] = Summoner.enemies_alive[i].animator;
        //}

        // update the node index and speed of each enemy using for loop
        for (int i = 0; i < Summoner.enemies_alive.Count; i++)
        {
            node_indexes[i] = Summoner.enemies_alive[i].Node_index;

            enemy_speeds[i] = Summoner.enemies_alive[i].speed;

            // check if the enemy is flying
            is_flying[i] = Summoner.enemies_alive[i].name.Contains("Jet");
            // also flies if contains drone
            is_flying[i] = is_flying[i] || Summoner.enemies_alive[i].name.Contains("Drone");
        }


        MoveJob moveJob = new MoveJob
        {
            node_index = node_indexes,
            enemy_speed = enemy_speeds,
            node_grid = to_use_nodes,
            deltaTime = Time.deltaTime,

            is_fly = is_flying
        };

        JobHandle Move_handle = moveJob.Schedule(enemy_access); // this will schedule the job to run on the enemy_access transforms
        Move_handle.Complete(); // this will complete the job



        // set all the data to the enemies
        for (int i = 0; i < Summoner.enemies_alive.Count; i++)
        {
            Enemy enemy = Summoner.enemies_alive[i];
            //Vector3 previousPosition = enemy.transform.position; //

            enemy.Node_index = node_indexes[i];
            // check if the enemy has reached the end of the path
            if (enemy.Node_index >= node_grid.Length)
            {
                // remove the enemy using enqueue_enemy_to_kill function
                enqueue_enemy_to_kill(enemy);
            }

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%



            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            enemy.speed = enemy_speeds[i];


        }

        //foreach (var enemy in Summoner.enemies_alive)
        //{
        //    if (enemy.animator == null)
        //    {
        //        Debug.LogError($"Animator missing on enemy {enemy.name}");
        //    }
        //    else if (!enemy.animator.isActiveAndEnabled)
        //    {
        //        Debug.LogError($"Animator not active on enemy {enemy.name}");
        //    }
        //}


        // dispose the arrays
        node_indexes.Dispose();
        enemy_speeds.Dispose();
        to_use_nodes.Dispose();
        enemy_access.Dispose();

        is_flying.Dispose();

        //animators.Dispose();





    }

    void ApplyEffects()
    {
        // Implement status effect applications here
    }

    void RemoveEnemies()
    {
        // Implement logic to remove defeated or exited enemies here
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
}


public struct MoveJob : IJobParallelForTransform
{
    [NativeDisableParallelForRestriction]
    public NativeArray<int> node_index;
    [NativeDisableParallelForRestriction]
    public NativeArray<float> enemy_speed;
    //[NativeDisableParallelForRestriction]
    //public NativeArray<Vector3> to_use_nodes;
    [NativeDisableParallelForRestriction]
    public NativeArray<Vector3> node_grid;

    //[NativeDisableParallelForRestriction]
    //public NativeArray<Animator> animators;

    [NativeDisableParallelForRestriction]
    public NativeArray<bool> is_fly;

    public float deltaTime;

    public void Execute(int index, TransformAccess transform)
    {
        // Adjust y position for "Jet" enemies

        //Debug.Log($"Moving enemy at index {index}");
        // check if the enemy has reached the end of the path
        if (node_index[index] >= node_grid.Length) // so only move the enemy if it has not reached the end of the path
        {
            return;
        }
        else
        {
            // Implement enemy movement logic here
            // Move the enemy towards the next node
            Vector3 move_direction = node_grid[node_index[index]];

            if (is_fly[index])
            {
                //change the y in move_direction to 2
                move_direction.y = 2;

                // change current position y to 2 and randomize x and z a bit
                transform.position = new Vector3(transform.position.x, 2, transform.position.z);

            }
            else
            {
                //change the y in move_direction to 0
                move_direction.y = 0;
            }







            transform.position = Vector3.MoveTowards(transform.position, move_direction, enemy_speed[index] * deltaTime);

            // Rotate the enemy to face the movement direction
            Vector3 direction = move_direction - transform.position;
            if (direction != Vector3.zero) // Prevent rotation when the position matches the move_direction
            {
                // rotation speed is based on the angle between the forward vector and the direction to the next node
                float rotation_speed = 0.05f * Vector3.Angle(Vector3.forward, direction) / 180; // 0.1f is the base rotation speed
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotation_speed);

            }


            // Check if the enemy has reached the node
            //if (Vector3.Distance(transform.position, move_direction) < 0.1f)
            if (transform.position == move_direction)
            {
                // Increment the node index
                //node_index[index] = (node_index[index] + 1) % node_grid.Length;
                node_index[index] = node_index[index] + 1;
                //Debug.Log($"Enemy reached node {node_index[index]}");

                // reset the rotation of the enemy
                transform.rotation = Quaternion.identity;

            }

            //if (is_fly[index])
            //{
            //    Vector3 adjustedPosition = transform.position;
            //    adjustedPosition.y = 2; // Set y to 2
            //    transform.position = adjustedPosition;
            //}



        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Summoner : MonoBehaviour
{
    public static Dictionary<int, GameObject> enemy_prefabs;
    public static Dictionary<int, Queue<Enemy>> enemy_pools;
    public static List<Enemy> enemies_alive;
    public static List<Transform> enemies_alive_transform;
    private static bool initialized;

    public static void Init()
    {
        if (!initialized)
        {
            enemy_prefabs = new Dictionary<int, GameObject>();
            enemy_pools = new Dictionary<int, Queue<Enemy>>();
            enemies_alive_transform = new List<Transform>();
            enemies_alive = new List<Enemy>();

            // get an array of all the enemies
            Enemy_spawn_data[] enemies = Resources.LoadAll<Enemy_spawn_data>("Enemies"); // (this is a folder in the resources folder)
            Debug.Log("There are " + enemies.Length + " enemies in the resources folder");
            Debug.Log("The first enemy is called " + enemies[0].name + " and has an ID of " + enemies[0].Enemy_ID);

            // populate the enemy_prefabs dictionary
            foreach (Enemy_spawn_data enemy in enemies)
            {
                enemy_prefabs.Add(enemy.Enemy_ID, enemy.enemy_prefab);
                enemy_pools.Add(enemy.Enemy_ID, new Queue<Enemy>());
            }

            initialized = true;
        }
    }

    public static Enemy spawn_enemy(int Enimy_ID)
    {
        Enemy Spawned_enemy = null;

        // Check if the ID is in the dictionary
        if (enemy_prefabs.ContainsKey(Enimy_ID))
        {
            Queue<Enemy> enemy_in_queue = enemy_pools[Enimy_ID]; // Pool of the enemy with the ID

            if (enemy_in_queue.Count > 0) // Spawn from pool
            {
                Spawned_enemy = enemy_in_queue.Dequeue(); // Get the enemy from the pool
                // if the object is destroyed, re-instantiate it
                if (Spawned_enemy == null)
                {
                    GameObject new_spawned = Instantiate(enemy_prefabs[Enimy_ID], EnemyManager.node_grid[0], Quaternion.identity);
                    new_spawned.name = "Orc_Enemy";
                    new_spawned.tag = "Orc";
                    Spawned_enemy = new_spawned.GetComponent<Enemy>();
                }
                else
                { // Reset the position
                    Spawned_enemy.transform.position = EnemyManager.node_grid[0];
                    Spawned_enemy.Node_index = 0;
                    Spawned_enemy.health = Spawned_enemy.max_health;
                    Spawned_enemy.isAlive = true;
                    Spawned_enemy.healthBar.UpdateHealth(Spawned_enemy.health, Spawned_enemy.max_health);
                    Spawned_enemy.gameObject.SetActive(true);
                }
            }
            else // Spawn new enemy
            {
                GameObject new_spawned = Instantiate(enemy_prefabs[Enimy_ID], EnemyManager.node_grid[0], Quaternion.identity);
                new_spawned.name = "Orc_Enemy";
                new_spawned.tag = "Orc";
                Spawned_enemy = new_spawned.GetComponent<Enemy>();
            }

            // Initialize the enemy
            Spawned_enemy.Init(); // Ensure Init() is called in both cases
            Debug.Log($"Enemy {Spawned_enemy.ID} spawned and initialized.");
        }
        else
        {
            Debug.LogError("Enemy ID " + Enimy_ID + " not found");
            return null;
        }

        // Add to active lists
        enemies_alive_transform.Add(Spawned_enemy.transform);
        enemies_alive.Add(Spawned_enemy);
        Spawned_enemy.ID = Enimy_ID;

        return Spawned_enemy;
    }

    // now removing the enemy from the scene
    public static void remove_enemy(Enemy enemy_to_kill)
    {
        enemy_pools[enemy_to_kill.ID].Enqueue(enemy_to_kill);
        enemy_to_kill.gameObject.SetActive(false);
        enemies_alive_transform.Remove(enemy_to_kill.transform);
        enemies_alive.Remove(enemy_to_kill);

    }

}

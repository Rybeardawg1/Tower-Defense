using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Summoner : MonoBehaviour
{
    public static Dictionary<int, GameObject> enemy_prefabs; // int is the ID of the enemy, GameObject is the prefab of the enemy
    public static Dictionary<int, Queue<Enemy>> enemy_pools; // int is the ID of the enemy, Queue<Enemy> is the pool of the enemy
    public static List<Enemy> enemies_alive; // list of enemies that are in the scene
    public static List<Transform> enemies_alive_transform; // to move the enemies in the scene

    private static bool initialized;
    
    public static void Init()
    {
        if (!initialized)
        {
            enemy_prefabs = new Dictionary<int, GameObject>();
            enemy_pools = new Dictionary<int, Queue<Enemy>>();
            enemies_alive_transform = new List<Transform>();
            enemies_alive = new List<Enemy>();

            // get an array of all the enemies in the resources folder (type Enemy_spawn_data)
            Enemy_spawn_data[] enemies = Resources.LoadAll<Enemy_spawn_data>("Enemies"); // (this is a folder in the resources folder)
            Debug.Log("There are " + enemies.Length + " enemies in the resources folder");
            Debug.Log("The first enemy is called " + enemies[0].name + " and has an ID of " + enemies[0].Enemy_ID);

            // populate the enemy_prefabs dictionary to have the enemy ID as the key and the enemy prefab as the value
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


        // check if the ID is in the dictionary
        if (enemy_prefabs.ContainsKey(Enimy_ID))
        {
            Queue<Enemy> enemy_in_queue = enemy_pools[Enimy_ID];// enemy_in_queue is the pool of the enemy with the ID Enimy_ID

            if (enemy_in_queue.Count > 0) // if there are enemies in the pool waiting to be spawned
            {
                Spawned_enemy = enemy_in_queue.Dequeue();// get the enemy from the pool
                Spawned_enemy.Init();// initialize the enemy

                // reactivate units that were deactivated in the remove_enemy function below (this way we can reuse the enemy)
                Spawned_enemy.gameObject.SetActive(true);
            }

            else
            {
                GameObject new_spawned = Instantiate(enemy_prefabs[Enimy_ID], EnemyManager.node_grid[0], Quaternion.identity);// instantiate the enemy prefab at the first node
                Spawned_enemy = new_spawned.GetComponent<Enemy>();// get the Enemy component of the enemy prefab
            }
        }

        else
        {
            Debug.LogError("Enemy ID " + Enimy_ID + " not found");
            return null;
        }

        enemies_alive_transform.Add(Spawned_enemy.transform);// add the enemy to the list of transforms of enemies alive for movement
        enemies_alive.Add(Spawned_enemy);// add the enemy to the list of enemies alive
        Spawned_enemy.ID = Enimy_ID;// set the ID of the enemy


        // start the enemy moving
        //Spawned_enemy.StartMoving();

        return Spawned_enemy;


    }

    // now removing the enemy from the scene
    public static void remove_enemy(Enemy enemy_to_kill)
    {
        enemy_pools[enemy_to_kill.ID].Enqueue(enemy_to_kill);
        enemy_to_kill.gameObject.SetActive(false);// set the enemy to inactive
        enemies_alive_transform.Remove(enemy_to_kill.transform);// remove the enemy from the list of transforms of enemies alive
        enemies_alive.Remove(enemy_to_kill);// remove the enemy from the list of enemies alive

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Enemy : MonoBehaviour
{
    public int Node_index; // for the pathfinding
    public float max_health;
    public float health;
    public float speed;
    public int ID;

    //public Animator animator;

    // initialize 
    public void Init()
    {
        max_health = 20;
        health = max_health;
        Debug.Log($"{gameObject.name} initialized with {health} health.");

        // set the position of the enemy to the first node
        transform.position = EnemyManager.node_grid[0];
        Node_index = 0; // to reset the node index to the first node when the enemy is spawned



    }




    public void TakeDamage(int damage)
    {   
        health -= damage;
        //if (health <= 0)
        //{
        //    //Destroy(gameObject); // Destroy enemy when health reaches zero
        //}
    }


}

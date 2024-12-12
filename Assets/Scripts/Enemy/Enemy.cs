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
        health = max_health;

        // set the position of the enemy to the first node
        transform.position = EnemyManager.node_grid[0];
        Node_index = 0; // to reset the node index to the first node when the enemy is spawned


        // Automatically find the Animator on this GameObject
        //if (animator == null) // Check if it's unassigned
        //{
        //    animator = GetComponent<Animator>();
        //    if (animator == null)
        //    {
        //        Debug.LogError($"Animator not found on {gameObject.name}");
        //    }
        //}
        //animator = GetComponentInChildren<Animator>();
        //animator = GetComponentInChildren<Animator>();
        //// Debug 
        //Debug.Log("Enemy initialized");
        //if (animator == null)
        //{
        //    Debug.LogError("No animator found on enemy");
        //}
        //else
        //{
        //    Debug.Log("Animator found on enemy");
        //}


    }

    //public void StartMoving()
    //{
    //    if (animator != null)
    //    {
    //        animator.SetBool("is_walking", true);
    //    }
    //    else
    //    {
    //        Debug.LogError("No animator found on enemy");
    //    }
    //}





}

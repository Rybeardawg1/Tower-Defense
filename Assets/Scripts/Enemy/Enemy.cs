using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Enemy : MonoBehaviour
{
    public int Node_index; // for the pathfinding
    public float max_health;
    public float speed;
    public int ID;
    private Animator animation_controller;
    private CharacterController character_controller;
    private float health;

    // initialize 
    public void Init()
    {

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

    void Start() {
        health = max_health;
        animation_controller = GetComponent<Animator>();
        character_controller = GetComponent<CharacterController>();

    }

    void Update() {
        if (health == 0) {
            animation_controller.SetBool("Die", true);
            animation_controller.SetBool("Walk", false);
        } else {
            animation_controller.SetBool("Die", false);
            animation_controller.SetBool("Walk", true);
        }
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Enemy : MonoBehaviour
{
    public int Node_index; // for the pathfinding
    public float max_health;
    public float speed;
    public int ID;
    public bool isAlive = true;
    private Animator animation_controller;
    private CharacterController character_controller;
    private float health;

    // initialize 
    public void Init()
    {

        // set the position of the enemy to the first node
        transform.position = EnemyManager.node_grid[0];
        Node_index = 0; // to reset the node index to the first node when the enemy is spawned


    }

    void Start() {
        health = max_health;
        animation_controller = GetComponent<Animator>();
        character_controller = GetComponent<CharacterController>();
        isAlive = true;
    }

    void Update() {
        if (health <= 0) 
        {
            isAlive = false;
            animation_controller.SetBool("Die", true);
            animation_controller.SetBool("Walk", false);
            Destroy(gameObject, 1.5f);
        }
        //else 
        //{
        //    animation_controller.SetBool("Die", false);
        //    animation_controller.SetBool("Walk", true);
        //}
        
        Perform_movement();
    }

    ////////// MM: new (revise later )
    public void Perform_movement()
    {
        if (isAlive)
        {
            Move_on_path();
        }
    }
    void Move_on_path()
    {
        if (Node_index < EnemyManager.node_grid.Length)
        {
            Vector3 targetNode = EnemyManager.node_grid[Node_index];
            Vector3 moveDirection = (targetNode - transform.position).normalized;
            transform.position = Vector3.MoveTowards(transform.position, targetNode, speed * Time.deltaTime);

            // Rotate to face movement direction
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }

            // Advance to next node if close enough
            if (Vector3.Distance(transform.position, targetNode) < 0.1f)
            {
                Node_index++;
            }
        }
        else
        {
            // Reached end of path, remove enemy
            EnemyManager.enqueue_enemy_to_kill(this);
        }
    }
    //////////

    // som/ash
    public void TakeDamage(int damage)
    {
        health -= damage;
    }
    //


}

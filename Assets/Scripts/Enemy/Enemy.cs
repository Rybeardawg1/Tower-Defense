using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Enemy : MonoBehaviour
{
    public int Node_index; // for the pathfinding
    public float max_health;
    public float speed;
    public int ID;
    public bool isAlive = true;
    private Animator animation_controller;
    private CharacterController character_controller;
    public float health;


    [Header("Sound Effects")]
    public AudioClip hit_sound;      // Sound for getting hit
    public AudioClip reached_goal_sound;      // Sound for reaching the end

    private AudioSource sfx_source;    // For sound effects
    private GameManager gameManager;


    //public AudioClip hitsound;
    //private AudioSource audioSource;

    [SerializeField] HealthBar healthBar;

    private void Awake()
    {
        healthBar = GetComponentInChildren<HealthBar>();
        // fetch the slider component from the health bar object

        GameObject game_manager = GameObject.FindGameObjectWithTag("GameManager");
        gameManager = game_manager.GetComponent<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("Internal error: could not find the GameManager object - did you remove its 'GameManager' tag?");
            return;
        }
    }

    // initialize 
    public void Init()
    {
        // set the position of the enemy to the first node
        transform.position = EnemyManager.node_grid[0];
        Node_index = 0; // to reset the node index to the first node when the enemy is spawned

    }
    void Start()
    {
        health = max_health;
        animation_controller = GetComponent<Animator>();
        character_controller = GetComponent<CharacterController>();
        isAlive = true;
        healthBar.UpdateHealth(health, max_health);

        sfx_source = gameObject.AddComponent<AudioSource>();
        sfx_source.loop = false;
        sfx_source.playOnAwake = false;
        sfx_source.volume = 0.5f;
    }

    void Update()
    {
            if (health <= 0 && isAlive)
            {
                health = 0;
                isAlive = false;
                animation_controller.SetBool("Die", true);
                animation_controller.SetBool("Walk", false);
                gameManager.UpdateBalance(20);
                Destroy(gameObject, 1.5f);
            }
        //else 
        //{
        //    animation_controller.SetBool("Die", false);
        //    animation_controller.SetBool("Walk", true);
        //}

        // Update health bar value
        healthBar.UpdateHealth(health, max_health);
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
            gameManager.ReduceGameHealth(5);
            if (reached_goal_sound != null)
            {
                Debug.Log("Playing Reached Goal Sound");
                sfx_source.PlayOneShot(reached_goal_sound);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isAlive)
        {
            health -= damage;
            healthBar.UpdateHealth(health, max_health);

            if (hit_sound != null)
            {
                sfx_source.PlayOneShot(hit_sound);
            }

            if(health <= 0){
                health = 0;
            }
        }
    }
}

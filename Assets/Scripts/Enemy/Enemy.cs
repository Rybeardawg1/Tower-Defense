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


    //private GameObject HealthBar;
    //private Slider health_bar_slider;

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
//         max_health = 20;
//         health = max_health;
//         Debug.Log($"{gameObject.name} initialized with {health} health.");
        // set the position of the enemy to the first node
        transform.position = EnemyManager.node_grid[0];
        Node_index = 0; // to reset the node index to the first node when the enemy is spawned
        
    }







    void Start() {
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

        // Update health bar value
        //if (health_bar_slider != null)
        //{
        //    // Make the health bar face the camera
        //    health_bar_slider.transform.parent.LookAt(Camera.main.transform);
        //    health_bar_slider.transform.parent.Rotate(0, 180, 0); // Correct for default backward orientation

        //    // Update the slider value (health)
        //    health_bar_slider.value = health / max_health;
        //}
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
            gameManager.UpdateGameHealth(-5);
            if (reached_goal_sound != null)
            {
                Debug.Log("Playing Reached Goal Sound");
                sfx_source.PlayOneShot(reached_goal_sound);
            }
        }
    }
    //////////

    // som/ash
    public void TakeDamage(int damage)
    {
        health -= damage;
        healthBar.UpdateHealth(health, max_health);

        if (hit_sound != null)
        {
            sfx_source.PlayOneShot(hit_sound);
        }

        if (health <= 0)
        {
            //Destroy(health_bar_slider.gameObject); // Remove health bar

            health = 0;
        }
    }
    //


}

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

    [SerializeField] HealthBar healthBar;


    //private GameObject HealthBar;
    //private Slider health_bar_slider;

    private void Awake()
    {
        healthBar = GetComponentInChildren<HealthBar>();
        // fetch the slider component from the health bar object
    }

    // initialize 
    public void Init()
    {

        // set the position of the enemy to the first node
        transform.position = EnemyManager.node_grid[0];
        Node_index = 0; // to reset the node index to the first node when the enemy is spawned



        //CreateHealthBar();
        //Debug.Log("Enemy initialized with health bar.");
    }

    //private void CreateHealthBar()
    //{
    //    // Create a new GameObject for the health bar
    //    GameObject canvasGO = new GameObject("HealthBarCanvas");
    //    Canvas canvas = canvasGO.AddComponent<Canvas>();
    //    canvas.renderMode = RenderMode.WorldSpace;
    //    canvas.transform.SetParent(transform, false);

    //    // Scale and position the canvas
    //    RectTransform canvasRect = canvas.GetComponent<RectTransform>();
    //    canvasRect.sizeDelta = new Vector2(4, 1f); // Size of the health bar
    //    canvasRect.localPosition = new Vector3(0, 4, 0); // Position above the enemy
    //    canvasRect.localScale = Vector3.one * 0.01f; // Small enough for a health bar

    //    // Create a slider for the health bar
    //    GameObject sliderGO = new GameObject("HealthBar");
    //    sliderGO.transform.SetParent(canvasGO.transform, false);



    //    health_bar_slider = sliderGO.AddComponent<Slider>();
    //    health_bar_slider.minValue = 0;
    //    health_bar_slider.maxValue = 1;
    //    health_bar_slider.value = 1;

    //    // Remove unnecessary background
    //    GameObject backgroundGO = new GameObject("Background");
    //    backgroundGO.transform.SetParent(sliderGO.transform, false);
    //    RectTransform bgRect = backgroundGO.AddComponent<RectTransform>();
    //    bgRect.anchorMin = Vector2.zero;
    //    bgRect.anchorMax = Vector2.one;
    //    bgRect.offsetMin = Vector2.zero;
    //    bgRect.offsetMax = Vector2.zero;

    //    Image background = backgroundGO.AddComponent<Image>();
    //    background.color = Color.black; // Black background for contrast
    //    health_bar_slider.targetGraphic = background;

    //    // Create the Fill Area
    //    GameObject fillGO = new GameObject("Fill");
    //    fillGO.transform.SetParent(sliderGO.transform, false);

    //    RectTransform fillRect = fillGO.AddComponent<RectTransform>();
    //    fillRect.anchorMin = Vector2.zero;
    //    fillRect.anchorMax = Vector2.one;
    //    fillRect.offsetMin = Vector2.zero;
    //    fillRect.offsetMax = Vector2.zero;

    //    Image fillImage = fillGO.AddComponent<Image>();
    //    fillImage.color = Color.red; // Green fill for health

    //    health_bar_slider.fillRect = fillRect;

    //    // Disable interaction for the slider (health bars are not clickable)
    //    health_bar_slider.interactable = false;

    //    // Debugging Logs
    //    Debug.Log("Health bar created");
    //    Debug.Log($"Canvas Rect: {canvasRect.sizeDelta}, Position: {canvasRect.localPosition}");
    //}






    void Start() {
        health = max_health;
        animation_controller = GetComponent<Animator>();
        character_controller = GetComponent<CharacterController>();
        isAlive = true;
        healthBar.UpdateHealth(health, max_health);
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
        }
    }
    //////////

    // som/ash
    public void TakeDamage(int damage)
    {
        health -= damage;
        healthBar.UpdateHealth(health, max_health);
        if (health <= 0)
        {
            //Destroy(health_bar_slider.gameObject); // Remove health bar

            health = 0;
        }
    }
    //


}

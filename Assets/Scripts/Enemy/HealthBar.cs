using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider health_bar_slider;
    [SerializeField] private Camera main_camera;
    [SerializeField] private Transform target; // this is the enemy object itself that the health bar is attached to
    // Start is called before the first frame update
    void Start()
    {
        health_bar_slider = GetComponent<Slider>();
        main_camera = Camera.main;

    }



    public void UpdateHealth(float health, float max_health)
    {
        health_bar_slider.value = health / max_health;
    }
    // Update is called once per frame
    void Update()
    {
        //transform.rotation = main_camera.transform.rotation; //didnt work
        transform.position = target.position + new Vector3(0, 4f, 0);

    }
}

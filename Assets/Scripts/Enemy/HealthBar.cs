using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider health_bar_slider;
    // Start is called before the first frame update
    void Start()
    {
        //health_bar_slider = GetComponent<Slider>();

    }

    // Update is called once per frame
    //void Update()
    //{

        
    //}

    public void UpdateHealth(float health, float max_health)
    {
        health_bar_slider.value = health / max_health;
    }
}

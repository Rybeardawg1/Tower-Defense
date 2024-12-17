using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text balanceText;
    public Text healthText;
    public int money;
    public int health;
    
    void Start()
    {
        money = 500;
        health = 150;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateBalance(int mon)
    {
        if (money + mon >= 0)
        {
            money += mon;
            balanceText.text = $"Balance: ${money}";
        }

    }

    public void ReduceGameHealth(int hel)
    {
        health -= hel;

        if (health <= 0)
        {
            health = 0; // Ensure health doesn’t go below 0
            healthText.text = $"Health: {health}";
            Debug.Log("Player's health reached 0.");
        }
        else
        {
            healthText.text = $"Health: {health}";
        }
    }
}

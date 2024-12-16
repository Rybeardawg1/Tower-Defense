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
    // Start is called before the first frame update
    void Start()
    {
        money = 500;
        health = 100;
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
        if (health - hel > 0)
        {
            health -= hel;
            healthText.text = $"Health: {health}";
        }
    }
}

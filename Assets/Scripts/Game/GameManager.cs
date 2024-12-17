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
    public GameObject RestartPanel;
    private bool isGameRunning;
    private MenuControl menuControl;

    void Start()
    {
        money = 180;
        health = 100;
        isGameRunning = true;
        RestartPanel.gameObject.SetActive(false);
        menuControl = gameObject.GetComponent<MenuControl>();
        UpdateBalance(0);
        ReduceGameHealth(0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateBalance(int mon)
    {
        if (isGameRunning && money + mon >= 0)
        {
            money += mon;
            balanceText.text = $"Balance: ${money}";
        }

    }

    public void ReduceGameHealth(int hel)
    {
        if (isGameRunning)
        {
            health -= hel;

            if (health <= 0)
            {
                isGameRunning = false;
                health = 0; // Ensure health doesn’t go below 0
                healthText.text = $"Health: {health}";
                ShowRestartPanel();
            }
            else
            {
                healthText.text = $"Health: {health}";
            }
        }
    }

    public void ShowRestartPanel()
    {
        menuControl.pauseGame();
        menuControl.playButton.gameObject.SetActive(false);
        menuControl.pauseButton.gameObject.SetActive(false);
        RestartPanel.gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name, LoadSceneMode.Single);
    }

}

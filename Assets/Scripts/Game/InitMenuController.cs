using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InitMenuController : MonoBehaviour
{
    public Button startButton;
    public TextMesh balanceText;
    public int money;
    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(StartGame);
        money = 500;
    }

    void StartGame()
    {
        SceneManager.LoadScene("Scene with enemies", LoadSceneMode.Single);
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
}

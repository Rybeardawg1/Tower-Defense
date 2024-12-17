using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InitMenuController : MonoBehaviour
{
    public Button startButton;
    public TextMesh balanceText;
    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(StartGame);
    }

    void StartGame()
    {
        SceneManager.LoadScene("Scene with enemies", LoadSceneMode.Single);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

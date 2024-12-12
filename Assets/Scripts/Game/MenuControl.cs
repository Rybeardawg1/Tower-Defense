using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuControl : MonoBehaviour
{
    public Button pauseButton;
    public Button playButton;

    private bool isPaused = false;
    // Start is called before the first frame update
    void Start()
    {
        pauseButton.gameObject.SetActive(true);
        playButton.gameObject.SetActive(false);
        pauseButton.onClick.AddListener(pauseGame);
        playButton.onClick.AddListener(playGame);
    }

    void pauseGame() {
        isPaused = true;
        Time.timeScale = 0;
        pauseButton.gameObject.SetActive(false);
        playButton.gameObject.SetActive(true);
    }

    void playGame() {
        isPaused = false;
        Time.timeScale = 1;
        pauseButton.gameObject.SetActive(true);
        playButton.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

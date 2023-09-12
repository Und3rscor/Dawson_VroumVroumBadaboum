using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public GameObject gameUI, gameOverUI, pauseUI;

    //Speedometer variables
    [HideInInspector] public float speedometer;
    private TextMeshProUGUI speedometerDisplay;
    private Slider speedometerSlider;

    //Lap counter variables
    [HideInInspector] public int lapCounter = 0;
    [HideInInspector] public bool lapAvailable;
    private TextMeshProUGUI lapCounterDisplay;

    //Scoreboard variables
    [HideInInspector] public int score = 0;
    private TextMeshProUGUI scoreboard;

    //Nos counter variables
    [HideInInspector] public float nosCounter;
    private Slider nosCounterSlider;

    //Health counter variables
    [HideInInspector] public int healthCounter;
    private Slider healthCounterSlider;

    //Lives counter variables
    [HideInInspector] public int livesCounter;
    private TextMeshProUGUI livesCounterDisplay;

    //Alive counter variable
    private TextMeshProUGUI aliveCounter;

    //Respawn Timer variables
    [HideInInspector] public int respawnTimer;
    private TextMeshProUGUI respawnTimerDisplay;

    private void Start()
    {
        if (gameUI != null)
        {
            gameUI.SetActive(true);

            //Find speedometer components
            speedometerDisplay = transform.Find("GameUI/Speedometer").GetComponentInChildren<TextMeshProUGUI>();
            speedometerSlider = transform.Find("GameUI/Speedometer").GetComponentInChildren<Slider>();

            //Find lap counter component
            lapCounterDisplay = transform.Find("GameUI/LapCounter").GetComponentInChildren<TextMeshProUGUI>();

            //Find scoreboard component
            scoreboard = transform.Find("GameUI/Scoreboard").GetComponentInChildren<TextMeshProUGUI>();

            //Starts the scoreboard
            if (!GameManager.Instance.GameOverBool)
            {
                Scoreboard();
            }

            //Find nos counter component
            nosCounterSlider = transform.Find("GameUI/NosCounter").GetComponentInChildren<Slider>();

            //Find health counter component
            healthCounterSlider = transform.Find("GameUI/HealthCounter").GetComponentInChildren<Slider>();

            //Find lives counter component
            livesCounterDisplay = transform.Find("GameUI/LivesCounter").GetComponentInChildren<TextMeshProUGUI>();

            //Find Alive Counter component
            aliveCounter = transform.Find("GameUI/AliveCounter").GetComponentInChildren<TextMeshProUGUI>();
        }

        if (gameOverUI != null)
        {
            //Find Respawn Timer component
            respawnTimerDisplay = transform.Find("GameOverUI/RespawnTimer").GetComponentInChildren<TextMeshProUGUI>();

            gameOverUI.SetActive(false);
        }

        if (pauseUI != null)
        {
            pauseUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (gameUI != null)
        {
            //Modify speedometer values
            speedometerDisplay.text = "Speed: " + Mathf.Round(speedometer) + " Km/h";
            speedometerSlider.value = speedometer;

            //Modify lap counter value
            lapCounterDisplay.text = "Lap: " + lapCounter;

            //Modify scoreboard value
            scoreboard.text = "Score: " + score;

            //Modify Nos Counter value
            nosCounterSlider.value = nosCounter;

            //Modify Health Counter value
            healthCounterSlider.value = healthCounter;

            //Modify Lives Counter value
            livesCounterDisplay.text = livesCounter.ToString();

            //Modify Alive Counter value
            aliveCounter.text = "Alive: " + GameManager.Instance.Alive + " / " + GameManager.Instance.TotalAlive;
        }

        if (gameOverUI != null)
        {
            //Modify Respawn Timer value
            if (respawnTimer > 1)
            {
                respawnTimerDisplay.text = "Respawning in: " + respawnTimer + " seconds";
            }
            else
            {
                respawnTimerDisplay.text = "Respawning in: " + respawnTimer + " second";
            }
            
        }
    }

    public void UIRedraw(GameObject active)
    {
        if (gameUI != null)
        {
            gameUI.SetActive(false);
        }

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }

        if (pauseUI != null)
        {
            pauseUI.SetActive(false);
        }

        if (active != null)
        {
            active.SetActive(true);
        }
    }
    public void Lap()
    {
        lapCounter++;
        score += 100;
    }

    public void Kill()
    {
        score += 1000;
    }

    private void Scoreboard()
    {
        score += (int)speedometer / 10;

        Invoke("Scoreboard", 1.0f);
    }

    public void Restart()
    {
        GameManager.Instance.Restart();
    }

    public void MainMenu()
    {
        GameManager.Instance.MainMenu();
    }

    public void Resume()
    {
        GameManager.Instance.Resume();

        UIRedraw(gameUI);
    }
}

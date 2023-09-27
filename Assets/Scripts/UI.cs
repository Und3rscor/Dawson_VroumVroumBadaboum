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
    public int Speedometer { get { return speedometer; } set { speedometer = value; } }
    private int speedometer;
    
    private TextMeshProUGUI speedometerDisplay;
    private Slider speedometerSlider;

    //Lap counter variables
    public int LapCounter { get { return lapCounter; } set { lapCounter = value; } }
    private int lapCounter = 0;

    private TextMeshProUGUI lapCounterDisplay;

    //Scoreboard variables
    public int Score { get { return score; } set { score = value; } }
    private int score = 0;

    private TextMeshProUGUI scoreboard;

    //Nos counter variables
    public float NosCounter { get { return nosCounter; } set { nosCounter = value; } }
    private float nosCounter;

    private Slider nosCounterSlider;

    //Heat counter variables
    public float HeatCounter { get { return heatCounter; } set { heatCounter = value; } }
    private float heatCounter;

    private Slider heatCounterSlider;

    //Health counter variables
    public int HealthCounter { get { return healthCounter; } set { healthCounter = value; } }
    private int healthCounter;

    private Slider healthCounterSlider;

    //Lives counter variables
    public int LivesCounter { get { return livesCounter; } set { livesCounter = value; } }
    private int livesCounter;

    private TextMeshProUGUI livesCounterDisplay;

    //Respawn Timer variables
    public int RespawnTimer { get { return respawnTimer; } set { respawnTimer = value; } }
    private int respawnTimer;

    private TextMeshProUGUI respawnTimerDisplay;

    //Alive counter variable
    private TextMeshProUGUI aliveCounter;

    private void Start()
    {
        if (gameUI != null)
        {
            gameUI.SetActive(true);

            //Find components
            speedometerDisplay = transform.Find("GameUI/Speedometer").GetComponentInChildren<TextMeshProUGUI>();    //Speedometer Km/h
            speedometerSlider = transform.Find("GameUI/Speedometer").GetComponentInChildren<Slider>();              //Speedometer Slider
            lapCounterDisplay = transform.Find("GameUI/LapCounter").GetComponentInChildren<TextMeshProUGUI>();      //Lap counter
            scoreboard = transform.Find("GameUI/Scoreboard").GetComponentInChildren<TextMeshProUGUI>();             //Score display
            nosCounterSlider = transform.Find("GameUI/NosCounter").GetComponentInChildren<Slider>();                //Nos Slider
            heatCounterSlider = transform.Find("GameUI/HeatCounter").GetComponentInChildren<Slider>();              //Heat Slider
            healthCounterSlider = transform.Find("GameUI/HealthCounter").GetComponentInChildren<Slider>();          //Health Slider
            livesCounterDisplay = transform.Find("GameUI/LivesCounter").GetComponentInChildren<TextMeshProUGUI>();  //Lives counter
            aliveCounter = transform.Find("GameUI/AliveCounter").GetComponentInChildren<TextMeshProUGUI>();         //Alive counter

            //Starts the scoreboard
            if (!GameManager.Instance.GameOverBool)
            {
                Scoreboard();
            }
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
            //Modify values
            speedometerDisplay.text = "Speed: " + speedometer + " Km/h";                               //Speedometer Km/h
            speedometerSlider.value = speedometer;                                                                  //Speedometer Slider value
            lapCounterDisplay.text = "Lap: " + lapCounter;                                                          //Lap counter
            scoreboard.text = "Score: " + score;                                                                    //Score counter
            nosCounterSlider.value = nosCounter;                                                                    //Nos Slider value
            heatCounterSlider.value = heatCounter;                                                                  //Heat Slider value
            healthCounterSlider.value = healthCounter;                                                              //Health Slider value
            livesCounterDisplay.text = livesCounter.ToString();                                                     //Lives counter
            aliveCounter.text = "Alive: " + GameManager.Instance.Alive + " / " + GameManager.Instance.TotalAlive;   //Alive counter
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

    //Removed stuff for alpha
    /*
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
    */
}

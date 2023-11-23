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

    //Respawn Timer variables
    public int RespawnTimer { get { return respawnTimer; } set { respawnTimer = value; } }
    private int respawnTimer;

    private TextMeshProUGUI respawnTimerDisplay;

    //Blue Zone Damage Visuals variable
    private GameObject blueZoneDamageVisualsImg;

    private void Start()
    {
        if (gameUI != null)
        {
            gameUI.SetActive(true);

            //Find components
            speedometerDisplay = transform.Find("GameUI/Speedometer").GetComponentInChildren<TextMeshProUGUI>();    //Speedometer Km/h
            speedometerSlider = transform.Find("GameUI/Speedometer").GetComponentInChildren<Slider>();              //Speedometer Slider
            scoreboard = transform.Find("GameUI/Scoreboard").GetComponentInChildren<TextMeshProUGUI>();             //Score display
            nosCounterSlider = transform.Find("GameUI/NosCounter").GetComponentInChildren<Slider>();                //Nos Slider
            heatCounterSlider = transform.Find("GameUI/HeatCounter").GetComponentInChildren<Slider>();              //Heat Slider
            healthCounterSlider = transform.Find("GameUI/HealthCounter").GetComponentInChildren<Slider>();          //Health Slider
            blueZoneDamageVisualsImg = transform.Find("GameUI/BlueZoneDamageVisuals").gameObject;                   //BlueZone Damage Visual Image
            blueZoneDamageVisualsImg.SetActive(false);                                                              //Disables the BlueZoneDVI on start

            //Starts the scoreboard
            if (!RaceManager.Instance.GameOverBool)
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
            speedometerDisplay.text = "Speed: " + speedometer + " Km/h";                                            //Speedometer Km/h
            speedometerSlider.value = speedometer;                                                                  //Speedometer Slider value
            scoreboard.text = "Score: " + score;                                                                    //Score counter
            nosCounterSlider.value = nosCounter;                                                                    //Nos Slider value
            heatCounterSlider.value = heatCounter;                                                                  //Heat Slider value
            healthCounterSlider.value = healthCounter;                                                              //Health Slider value
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

    public void Checkpoint()
    {
        score += 50;
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

    public void BlueZoneDamageVisualsToggle(bool active)
    {
        if (blueZoneDamageVisualsImg != null)
        {
            blueZoneDamageVisualsImg.SetActive(active);
        }
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

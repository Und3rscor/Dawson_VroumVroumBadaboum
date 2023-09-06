using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public GameObject gameUI, gameOverUI, pauseUI;

    //Speedometer variables
    [HideInInspector] public float speedometer;
    private TextMeshProUGUI speedometerText;
    private Slider speedometerSlider;

    //Lap counter variable
    [HideInInspector] public int lapCounter = 0;
    [HideInInspector] public bool lapAvailable;
    private TextMeshProUGUI lapCounterTMP;

    //Scoreboard variable
    private TextMeshProUGUI scoreboard;

    //Alive counter variable
    private TextMeshProUGUI aliveCounter;

    //Nos counter variable
    private Slider nosCounter;

    private void Start()
    {
        if (gameUI != null)
        {
            gameUI.SetActive(true);

            //Find speedometer components
            speedometerText = transform.Find("GameUI/Speedometer/Text").GetComponent<TextMeshProUGUI>();
            speedometerSlider = transform.Find("GameUI/Speedometer/Slider").GetComponent<Slider>();

            //Find lap counter component
            lapCounterTMP = transform.Find("GameUI/LapCounter/Text").GetComponent<TextMeshProUGUI>();

            //Find scoreboard component
            scoreboard = transform.Find("GameUI/Scoreboard/Text").GetComponent<TextMeshProUGUI>();

            //Find nos counter component
            nosCounter = transform.Find("GameUI/NosCounter/Slider").GetComponent<Slider>();

            //Find Alive Counter component
            aliveCounter = transform.Find("GameUI/AliveCounter/Text").GetComponent<TextMeshProUGUI>();
        }

        if (gameOverUI != null)
        {
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
            speedometerText.text = "Speed: " + Mathf.Round(speedometer) + " Km/h";
            speedometerSlider.value = speedometer;

            //Modify lap counter value
            lapCounterTMP.text = "Lap: " + lapCounter;

            //Modify scoreboard value
            scoreboard.text = "Score: " + GameManager.Instance.Score;

            //Modify Nos Counter value
            nosCounter.value = GameManager.Instance.NosCounterGM;

            //Modify Alive Counter value
            aliveCounter.text = "Alive: " + GameManager.Instance.Alive + " / " + GameManager.Instance.TotalAlive;
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
        //score += 100;
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

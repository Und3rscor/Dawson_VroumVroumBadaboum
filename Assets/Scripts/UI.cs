using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField]
    private GameObject gameUI, gameOverUI;

    //Speedometer variables
    private TextMeshProUGUI speedometerText;
    private Slider speedometerSlider;

    //Lap counter variable
    private TextMeshProUGUI lapCounter;

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
            lapCounter = transform.Find("GameUI/LapCounter/Text").GetComponent<TextMeshProUGUI>();

            //Find scoreboard component
            scoreboard = transform.Find("GameUI/Scoreboard/Text").GetComponent<TextMeshProUGUI>();

            //Find Alive Counter component
            aliveCounter = transform.Find("GameUI/AliveCounter/Text").GetComponent<TextMeshProUGUI>();

            //Find nos counter component
            nosCounter = transform.Find("GameUI/NosCounter/Slider").GetComponent<Slider>();
        }

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (gameUI != null)
        {
            //Modify speedometer values
            speedometerText.text = "Speed: " + Mathf.Round(GameManager.Instance.SpeedometerGM) + " Km/h";
            speedometerSlider.value = GameManager.Instance.SpeedometerGM;

            //Modify lap counter value
            lapCounter.text = "Lap: " + GameManager.Instance.Laps;

            //Modify scoreboard value
            scoreboard.text = "Score: " + GameManager.Instance.Score;

            //Modify Alive Counter value
            aliveCounter.text = "Alive: " + GameManager.Instance.Alive + " / " + GameManager.Instance.TotalAlive;

            //Modify Nos Counter value
            nosCounter.value = GameManager.Instance.NosCounterGM;
        }
    }

    public void GameOverUIRedraw()
    {
        if (gameUI != null)
        {
            gameUI.SetActive(false);
        }

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
    }

    public void Restart()
    {
        GameManager.Instance.Restart();
    }

    public void MainMenu()
    {
        GameManager.Instance.MainMenu();
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    //Speedometer variables
    private TextMeshProUGUI speedometerText;
    private Slider speedometerSlider;

    //Lap counter variable
    private TextMeshProUGUI lapCounter;

    //Scoreboard variable
    private TextMeshProUGUI scoreboard;

    //Alive counter variable
    private TextMeshProUGUI aliveCounter;

    private void Start()
    {
        //Find speedometer components
        speedometerText = transform.Find("Speedometer/Text").GetComponent<TextMeshProUGUI>();
        speedometerSlider = transform.Find("Speedometer/Slider").GetComponent<Slider>();

        //Find lap counter component
        lapCounter = transform.Find("LapCounter/Text").GetComponent<TextMeshProUGUI>();

        //Find scoreboard component
        scoreboard = transform.Find("Scoreboard/Text").GetComponent<TextMeshProUGUI>();

        //Find Alive Counter component
        aliveCounter = transform.Find("AliveCounter/Text").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
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
    }
}

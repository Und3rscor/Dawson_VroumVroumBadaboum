using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;

public class Countdown : MonoBehaviour
{
    public string[] Messages; //3, 2, 1, Go!
    public float countdownDuration;
    private TextMeshProUGUI countdownText = null;
    private bool isCountdownRunning;

    private void Start()
    {
        countdownText = GetComponent<TextMeshProUGUI>();
        StartCoroutine(CountdownCoroutine());
        Time.timeScale = 0.0f;
    }

    public IEnumerator CountdownCoroutine()
    {
        float startTime = Time.realtimeSinceStartup;
        float elapsedTime = 0f;

        while (elapsedTime < countdownDuration)
        {
            yield return null;
            elapsedTime = Time.realtimeSinceStartup - startTime;

            if (Messages.Length > Mathf.FloorToInt(elapsedTime))
            {
                countdownText.text = Messages[Mathf.FloorToInt(elapsedTime)];
            }

            Debug.Log("Remaining time: " + (countdownDuration - elapsedTime));
        }

        Time.timeScale = 1.0f;
        gameObject.SetActive(false);
    }

    private void StartCountdown()
    {
        if (!isCountdownRunning)
        {
            StartCoroutine(CountdownCoroutine());
            isCountdownRunning = true;
        }
    }
}

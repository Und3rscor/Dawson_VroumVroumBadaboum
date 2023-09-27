using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class Countdown : MonoBehaviour
{
    public string[] Messages; //3, 2, 1, Go!
    public float countdownDuration;
    public AudioSource countdownSound;

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
                if (countdownText.text != Messages[Mathf.FloorToInt(elapsedTime)])
                {
                    countdownSound.Play();
                }

                if (countdownText.text == Messages[0])
                {
                    RaceManager.Instance.Setup();
                    //GameManager.Instance.FindUI();
                }

                countdownText.text = Messages[Mathf.FloorToInt(elapsedTime)];
            }
        }

        Time.timeScale = 1.0f;

        countdownSound.pitch = 1.5f;
        countdownSound.volume = 0.3f;
        countdownSound.Play();

        countdownText.text = null;
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

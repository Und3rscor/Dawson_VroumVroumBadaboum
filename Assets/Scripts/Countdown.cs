using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using DG.Tweening;

public class Countdown : MonoBehaviour
{
    public string[] Messages; //3, 2, 1, Go!
    public float countdownDuration;
    public AudioSource countdownSound;

    private TextMeshProUGUI countdownText = null;
    private bool isCountdownRunning;

    private void Start()
    {
        countdownText = GetComponentInChildren<TextMeshProUGUI>();
        StartCoroutine(CountdownCoroutine());
        RaceManager.Instance.Pause();
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

                countdownText.text = Messages[Mathf.FloorToInt(elapsedTime)];
            }
        }

        RaceManager.Instance.Resume();

        countdownSound.pitch = 1.5f;
        countdownSound.volume = 0.3f;
        countdownSound.Play();
    }

    public void StartCountdown()
    {
        if (!isCountdownRunning)
        {
            StartCoroutine(CountdownCoroutine());
            isCountdownRunning = true;
        }
    }
}

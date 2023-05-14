using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu, settingsMenu;
    [Range(0f, 1f)] public float volumeLevel = 0.5f;

    private Slider volumeSlider;

    private void Start()
    {
        RedrawMenu(mainMenu);
        volumeSlider = transform.Find("SettingsMenu/Menu/Audio/AudioSlider").GetComponent<Slider>();
    }

    private void RedrawMenu(GameObject active)
    {
        if (mainMenu != null)
        {
            mainMenu.SetActive(false);
        }
        if (settingsMenu != null)
        {
            settingsMenu.SetActive(false);
        }
        if (active != null)
        {
            active.SetActive(true);
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        AudioListener.volume = volumeLevel;
    }

    public void Settings()
    {
        RedrawMenu(settingsMenu);
        volumeSlider.value = volumeLevel;
    }

    public void Back()
    {
        RedrawMenu(mainMenu);
    }

    public void QuitGame()
    {
        Debug.Log("GameQuit");
        Application.Quit();
    }

    public void VolumeMixer(float value)
    {
        value = volumeLevel;
    }
}

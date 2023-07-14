using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] menus;
    [Range(0f, 1f)] public float volumeLevel = 0.5f;

    private Slider volumeSlider;

    private void Start()
    {
        RedrawMenu(menus[0]);
        volumeSlider = transform.Find("SettingsMenu/Menu/Audio/AudioSlider").GetComponent<Slider>();
    }

    private void RedrawMenu(GameObject active)
    {
        foreach (var menu in menus)
        {
            menu.SetActive(false);
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
        RedrawMenu(menus[1]);
        volumeSlider.value = volumeLevel;
    }

    public void Garage()
    {
        RedrawMenu(menus[2]);
    }

    public void Back()
    {
        RedrawMenu(menus[0]);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] menus;
    [Range(0f, 1f)] public float volumeLevel = 0.5f;

    public GameObject masterAudio;
    private Slider volumeSlider;

    private void Start()
    {
        RedrawMenu(menus[0]);
        volumeSlider = masterAudio.GetComponent<Slider>();
        //volumeSlider = transform.Find("SettingsMenu/Menu/Audio/AudioSlider").GetComponent<Slider>();
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

    #region Menus
    public void LandingScreen()
    {
        RedrawMenu(menus[0]);
        volumeSlider.value = volumeLevel;
    }

    public void HomeScreen()
    {
        RedrawMenu(menus[1]);
    }

    public void AssemblyScreen()
    {
        RedrawMenu(menus[2]);
    }

    public void SettingsGraphicScreen()
    {
        RedrawMenu(menus[3]);
        volumeSlider.value = volumeLevel;
    }

    public void SettingsControlsScreen()
    {
        RedrawMenu(menus[4]);
        volumeSlider.value = volumeLevel;
    }

    public void SettingsAudioScreen()
    {
        RedrawMenu(menus[5]);
        volumeSlider.value = volumeLevel;
    } 
    
    public void SettingsCreditsScreen()
    {
        RedrawMenu(menus[6]);
        volumeSlider.value = volumeLevel;
    }

    public void ExtrasScreen()
    {
        RedrawMenu(menus[7]);
        volumeSlider.value = volumeLevel;
    }

    public void LocalPlayScreen()
    {
        RedrawMenu(menus[8]);
        volumeSlider.value = volumeLevel;
    }

    #endregion



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

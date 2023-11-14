using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] menus;
    [Range(0f, 1f)] public float volumeLevel = 0.5f;

    public GameObject masterAudio;
    private Slider volumeSlider;

    // for the CTA
    public GameObject canevas2;

    [Header("First Selected Options")]
    [SerializeField] private GameObject homeScreenFirst;

    private void Start()
    {
        RedrawMenu(menus[0]);
        volumeSlider = masterAudio.GetComponent<Slider>();
        canevas2.SetActive(false);
        
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
        canevas2.SetActive(false);
    }

    public void HomeScreen()
    {
        RedrawMenu(menus[1]);
        canevas2.SetActive(true);

        EventSystem.current.SetSelectedGameObject(homeScreenFirst);
    }

    public void AssemblyScreen()
    {
        RedrawMenu(menus[2]);
        canevas2.SetActive(false);
    }

    public void SettingsGraphicScreen()
    {
        RedrawMenu(menus[3]);
        volumeSlider.value = volumeLevel;
        canevas2.SetActive(false);
    }

    public void SettingsControlsScreen()
    {
        RedrawMenu(menus[4]);
        volumeSlider.value = volumeLevel;
        canevas2.SetActive(false);
    }

    public void SettingsAudioScreen()
    {
        RedrawMenu(menus[5]);
        volumeSlider.value = volumeLevel;
        canevas2.SetActive(false);
    } 
    
    public void SettingsCreditsScreen()
    {
        RedrawMenu(menus[6]);
        volumeSlider.value = volumeLevel;
        canevas2.SetActive(false);
    }

    public void ExtrasScreen()
    {
        RedrawMenu(menus[7]);
        volumeSlider.value = volumeLevel;
        canevas2.SetActive(false);
    }

    public void LocalPlayScreen()
    {
        RedrawMenu(menus[8]);
        volumeSlider.value = volumeLevel;
        canevas2.SetActive(false);
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

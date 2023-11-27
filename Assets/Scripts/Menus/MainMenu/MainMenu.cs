using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEditor;
using UnityEngine.Windows;
using MoreMountains.Tools;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] menus;
    [Range(0f, 1f)] public float volumeLevel = 0.5f;

    public GameObject masterAudio;
    private Slider volumeSlider;

    private PlayerConfigManager pcm;
    private PlayerInputManager pim;

    public GameObject[] pressAs;

    private void Start()
    {
        pcm = PlayerConfigManager.Instance;
        pim = pcm.gameObject.GetComponent<PlayerInputManager>();

        RedrawMenu(menus[0]);
        volumeSlider = masterAudio.GetComponent<Slider>();

        GetComponent<Canvas>().worldCamera = FindObjectOfType<Camera>();

        PlayerConfigManager.Instance.pressAs = pressAs;
    }

    private void RedrawMenu(GameObject active)
    {
        pim.DisableJoining();

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

    public void HomeScreen()
    {
        RedrawMenu(menus[0]);
    }

    public void AssemblyScreen()
    {
        RedrawMenu(menus[1]);
    }

    public void SettingsGraphicScreen()
    {
        RedrawMenu(menus[2]);
        volumeSlider.value = volumeLevel;
    }

    public void SettingsControlsScreen()
    {
        RedrawMenu(menus[3]);
        volumeSlider.value = volumeLevel;
    }

    public void SettingsAudioScreen()
    {
        RedrawMenu(menus[4]);
        volumeSlider.value = volumeLevel;
    } 
    
    public void SettingsCreditsScreen()
    {
        RedrawMenu(menus[5]);
        volumeSlider.value = volumeLevel;
    }

    public void ExtrasScreen()
    {
        RedrawMenu(menus[6]);
        volumeSlider.value = volumeLevel;
    }

    public void LocalPlayScreen()
    {
        RedrawMenu(menus[7]);
        volumeSlider.value = volumeLevel;

        GetComponentInChildren<PlayerSetupMenuController>().SetPlayerIndex(0);
        pim.EnableJoining();
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

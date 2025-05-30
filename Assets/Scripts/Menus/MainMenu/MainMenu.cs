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
using UnityEngine.InputSystem.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] menus;
    [Range(0f, 1f)] public float volumeLevel = 0.5f;

    public GameObject masterAudio;
    private Slider volumeSlider;

    private PlayerConfigManager pcm;
    private PlayerInputManager pim;
    private MultiplayerEventSystem mEventSystem;

    public GameObject[] playerSlots;

    private void Awake()
    {
        mEventSystem = GetComponentInChildren<MultiplayerEventSystem>();
    }

    private void Start()
    {
        pcm = PlayerConfigManager.Instance;
        pim = pcm.gameObject.GetComponent<PlayerInputManager>();

        RedrawMenu(menus[0]);
        volumeSlider = masterAudio.GetComponent<Slider>();

        GetComponent<Canvas>().worldCamera = FindObjectOfType<Camera>();

        PlayerConfigManager.Instance.playerSlots = playerSlots;
    }

    public void RedrawMenu(GameObject active)
    {
        pim.DisableJoining();

        foreach (var menu in menus)
        {
            menu.SetActive(false);
        }

        if (active != null)
        {
            active.SetActive(true);

            GrabButton(active);
        }
    }

    public void PlayGame()
    {
        FixVisualGlitchBeforeLoad();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        AudioListener.volume = volumeLevel;
    }

    public void HomeScreen()
    {
        RedrawMenu(menus[0]);
    }

    public void LocalPlayScreen()
    {
        RedrawMenu(menus[2]);
        volumeSlider.value = volumeLevel;

        GetComponentInChildren<PlayerSetupMenuController>().SetPlayerIndex(0);
        pim.EnableJoining();
    }

    public void OptionScreen()
    {
        RedrawMenu(menus[1]);
    }

    private void FixVisualGlitchBeforeLoad()
    {
        RedrawMenu(menus[3]);
    }

    private void GrabButton(GameObject menu)
    {
        if (menu.GetComponentInChildren<Button>() != null)
        {
            mEventSystem.SetSelectedGameObject(menu.GetComponentInChildren<Button>().gameObject);
        }        
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu, settingsMenu;

    private void Start()
    {
        RedrawMenu(mainMenu);
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
    }

    public void Settings()
    {
        RedrawMenu(settingsMenu);
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
}

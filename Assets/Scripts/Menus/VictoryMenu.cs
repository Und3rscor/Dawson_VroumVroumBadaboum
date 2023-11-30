using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryMenu : MonoBehaviour
{
    public void ReturnToMainMenu()
    {
        PlayerConfigManager.Instance.NukeAllDDOL();

        SceneManager.LoadScene(0);
    }
}

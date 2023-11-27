using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class SpawnPlayerSetupMenu : MonoBehaviour
{
    public GameObject mainPlayerSetupMenuPrefab;
    public GameObject playerSetupMenuPrefab;
    public PlayerInput input;

    private void Awake()
    {
        var mainRootMenu = GameObject.FindGameObjectWithTag("MainMenu");
        if (mainRootMenu != null)
        {
            mainRootMenu.SetActive(false);
            var menu = Instantiate(mainPlayerSetupMenuPrefab);
            input.uiInputModule = menu.GetComponentInChildren<InputSystemUIInputModule>();
        }
        else
        {
            var rootmenu = GameObject.FindGameObjectWithTag("PlayerSelection");
            if (rootmenu != null)
            {
                GameObject original = playerSetupMenuPrefab;
                Vector3 position = PlayerConfigManager.Instance.pressAs[input.playerIndex - 1].transform.position;
                Quaternion rotation = PlayerConfigManager.Instance.pressAs[input.playerIndex - 1].transform.rotation;
                Transform parent = rootmenu.transform;

                var menu = Instantiate(original, position, rotation, parent);
                input.uiInputModule = menu.GetComponentInChildren<InputSystemUIInputModule>();
                menu.GetComponent<PlayerSetupMenuController>().SetPlayerIndex(input.playerIndex);

                PlayerConfigManager.Instance.pressAs[input.playerIndex - 1].SetActive(false);
            }
        }
    }
}

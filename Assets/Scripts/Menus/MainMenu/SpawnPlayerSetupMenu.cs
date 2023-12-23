using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class SpawnPlayerSetupMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainPlayerSetupMenuPrefab;
    [SerializeField] private GameObject playerSetupMenuPrefab;
    [SerializeField] private PlayerInput input;

    private void Awake()
    {
        var mainRootMenu = GameObject.FindGameObjectWithTag("MainMenu");
        if (mainRootMenu != null)
        {
            mainRootMenu.SetActive(false);
            var menu = Instantiate(mainPlayerSetupMenuPrefab);
            input.uiInputModule = menu.GetComponentInChildren<InputSystemUIInputModule>();
            PlayerConfigManager.Instance.GrabMenu(menu.GetComponent<MainMenu>());
        }
        else
        {
            var rootmenu = GameObject.FindGameObjectWithTag("PlayerSelection");
            if (rootmenu != null)
            {
                //Grab relevant player slot
                Transform playerSlot = PlayerConfigManager.Instance.playerSlots[input.playerIndex - 1].transform;

                //Grab "Press A"
                GameObject menuToReplace = playerSlot.transform.GetChild(0).gameObject;

                //Grab values for PlayerSetupMenu
                GameObject original = playerSetupMenuPrefab;
                Transform parent = playerSlot.transform;

                //Spawn PlayerSetupMenu
                var menu = Instantiate(original, parent);
                input.uiInputModule = menu.GetComponentInChildren<InputSystemUIInputModule>();
                menu.GetComponent<PlayerSetupMenuController>().SetPlayerIndex(input.playerIndex);

                //Remove "Press A"
                Destroy(menuToReplace);
            }
        }
    }
}

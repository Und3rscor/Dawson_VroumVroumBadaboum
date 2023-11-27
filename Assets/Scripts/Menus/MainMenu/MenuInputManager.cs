using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuInputManager : MonoBehaviour
{
    public static MenuInputManager instance;

    private PlayerInput playerInput;

    private InputAction homeScreenAction;

    private MainMenu mainMenu;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        playerInput = FindObjectOfType<PlayerInput>();

        mainMenu = FindObjectOfType<MainMenu>();
    }

    private void Update()
    {
        if (playerInput.actions["HomeScreen"].WasPressedThisFrame())
        {
            mainMenu.HomeScreen();
        }
    }
}

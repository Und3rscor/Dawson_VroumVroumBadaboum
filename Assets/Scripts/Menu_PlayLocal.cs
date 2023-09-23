using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_PlayLocal : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject buttonADisplay; // Ui Button to tell the players to press A
    public GameObject carSelectionMenu; // Reference to the car selection menu 
    public GameObject colorSelectionMenu; 
    public GameObject carDisplay;  //display of the selected car
    public GameObject inputSelection;

    [Header("Car Selection")]  
    [SerializeField] private GameObject[] carPrefabs;

    [Header("StartButton")]
    public Menu_StartLocalButton startButton;
    

    


    private void Start()
    {
        // Initially, hide the car selection menu
        carSelectionMenu.SetActive(false); 
        carDisplay.SetActive(false); 
        colorSelectionMenu.SetActive(false);
        buttonADisplay.SetActive(true);
        inputSelection.SetActive(false);


        RedrawMenu(carPrefabs[0]);

        
    }

    private void Update()
    {
        // Toggle the car selection menu's visibility
        if (Input.GetKeyDown(KeyCode.A))
        {            
            carSelectionMenu.SetActive(true);
            carDisplay.SetActive(true);
            buttonADisplay.SetActive(false);
            colorSelectionMenu.SetActive(true);

            ActivatePlayer();
        }
    }

    private void RedrawMenu(GameObject active)
    {
        foreach (var carprefab in carPrefabs)
        {
            carprefab.SetActive(false);
        }

        if (active != null)
        {
            active.SetActive(true);
        }
    }


    private void ActivatePlayer()
    {
        startButton.PlayerActivated();
    }

    public void PlayerIsReady()
    {
        startButton.PlayerReady();  //Tell start button that this player is ready


        // change the UI to input selection
        carSelectionMenu.SetActive(false);
        carDisplay.SetActive(false);
        buttonADisplay.SetActive(false);
        inputSelection.SetActive(true);
        colorSelectionMenu.SetActive(false);
    }

    /*   //Is see un gros bug incoming si c'est pas implémenté ultra bien, et j'ai la fléme so désactiver pour now.
    private void PlayerIsUnready()
    {
        startButton.PlayerUnready();
    }
    */

    #region carPrefabSelection

    public void AetherOne_Black()
    {
        RedrawMenu(carPrefabs[0]);          
    }

    public void AetherOne_Blue()
    {
        RedrawMenu(carPrefabs[1]);
    }

    public void AetherOne_Orange()
    {
        RedrawMenu(carPrefabs[2]);
    }

    public void AetherOne_Red()
    {
        RedrawMenu(carPrefabs[3]);
    }

    #endregion
}

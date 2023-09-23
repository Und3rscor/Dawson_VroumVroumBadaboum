using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_StartLocalButton : MonoBehaviour
{
    public GameObject startButton; // Reference to the "Start Game" button
    private int activatedPlayers = 0; // Count of activated players
    private int readyPlayers = 0; // Count of players who are ready
    private int totalPlayers = 4; // Adjust as needed for the maximum number of players

    private void Start()
    {
        startButton.SetActive(false); // Initially, disable the "Start Game" button
    }

    private void Update()
    {
        if (activatedPlayers == readyPlayers)
        {
            // You can add additional conditions if needed (e.g., minimum players ready)
            startButton.SetActive(true);
        }
        else
        {
            startButton.SetActive(false);
        }
            
    }


    public void PlayerActivated()
    {
        activatedPlayers++;
        
    }

    public void PlayerReady()
    {
        readyPlayers++;
        
    }

    public void PlayerUnready()
    {
        readyPlayers--;
       
    }


}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSetupMenuController : MonoBehaviour
{
    private int playerIndex;

    [SerializeField] private TextMeshProUGUI playerText;
    [SerializeField] private Button readyButton;
    [SerializeField] private GameObject carModel;
    private PlayerColorSetup playerColor;

    private void Awake()
    {
        playerColor = carModel.GetComponent<PlayerColorSetup>();

        if (tag == "MainPlayer")
        {
            playerIndex = 0;
        }
    }

    public void SetPlayerIndex(int pi)
    {
        playerIndex = pi;
        playerText.SetText("Player " + (pi + 1).ToString());
    }

    public void SetColor(Material color)
    {
        playerColor.SetupColor(color, Color.black);
        PlayerConfigManager.Instance.SetPlayerColor(playerIndex, color);
    }

    public void ReadyPlayer()
    {
        readyButton.interactable = false;

        PlayerConfigManager.Instance.ReadyPlayer(playerIndex);

        Debug.Log("Player " + playerIndex + " is ready!");
    }
}

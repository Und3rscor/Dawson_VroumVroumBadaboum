using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerConfig : MonoBehaviour
{
    public PlayerConfig(PlayerInput input)
    {
        Input = input;
        PlayerIndex = input.playerIndex;
    }

    public PlayerInput Input { get; set; }
    public int PlayerIndex { get; set; }
    public bool isReady { get; set; }
    public Material color { get; set; }
}

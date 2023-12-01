using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerConfig
{
    public PlayerConfig(PlayerInput input)
    {
        Input = input;
        PlayerIndex = input.playerIndex;
    }

    public PlayerInput Input { get; set; }
    public int PlayerIndex { get; set; }
    public bool IsReady { get; set; }
    public Material Mat { get; set; }

    public string Name { get; set; }
    public int Score { get; set; }
    public int Kills { get; set; }
    public bool FirstPlayer { get; set; }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerRelay : MonoBehaviour
{
    public PlayerInput CarInput { get { return carInput; } }
    private PlayerInput carInput;

    public void SetPlayerInput(PlayerInput input)
    {
        carInput = input;
    }
}

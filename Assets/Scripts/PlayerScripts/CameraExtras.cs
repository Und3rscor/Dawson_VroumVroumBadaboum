using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraExtras : MonoBehaviour
{
    //Camera controls
    [Header("Camera controls")]
    [SerializeField] private CinemachineVirtualCamera mainCam;
    [SerializeField] private CinemachineVirtualCamera reverseCam;

    private Camera camBrain;
    private PlayerInput playerInput;

    private void Start()
    {
        camBrain = GetComponent<Camera>();
        playerInput = GetComponentInParent<PlayerInput>();        
    }

    public void InitializePlayer(PlayerConfig pc)
    {
        //Asks the gamemange which layer mask is available for the cameras. Basically the player ID (player 1, 2...)
        RaceManager.Instance.PlayerSetup(this.gameObject, camBrain, pc);
    }

    private void LateUpdate()
    {
        CameraControls();
    }

    //Checks for Back Cam Input and then calls the turn cam function if pressed
    private void CameraControls()
    {
        if (playerInput.actions["Back Cam"].WasPressedThisFrame())
        {
            SwitchCam();
        }

        if (playerInput.actions["Back Cam"].WasReleasedThisFrame())
        {
            SwitchCam();
        }
    }

    //Changes the vcam priorities to quickly check behind you
    public void SwitchCam()
    {
        if (mainCam.Priority != 1)
        {
            mainCam.Priority = 1;
            reverseCam.Priority = 2;
        }
        else
        {
            mainCam.Priority = 2;
            reverseCam.Priority = 1;
        }
    }
}

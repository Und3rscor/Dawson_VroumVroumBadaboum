using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraExtras : MonoBehaviour
{
    //Camera controls
    [Header("Camera controls")]
    [SerializeField] private CinemachineVirtualCamera mainCam;
    [SerializeField] private CinemachineVirtualCamera reverseCam;
    [SerializeField] private KeyCode turnCamKey = KeyCode.LeftControl;
    [SerializeField] private bool cameraTurnHold = true;

    private Animator animator;
    private bool cinematicCamTurned = false;
    private bool cinematicCamTurn = false;
    public bool CinematicCamTurn { get { return cinematicCamTurn; } set { cinematicCamTurn = value; } }

    private Camera camBrain;

    private void Start()
    {
        animator = mainCam.GetComponent<Animator>();
        camBrain = GetComponent<Camera>();

        //Asks the gamemange which layer mask is available for the cameras. Basically the player ID (player 1, 2...)
        GameManager.Instance.CameraSetup(this.gameObject, camBrain);
    }

    private void Update()
    {
        if (cinematicCamTurn)
        {
            turnCamCinematic();
            animator.SetBool("Spin", cinematicCamTurned);
        }
    }

    private void LateUpdate()
    {
        CameraControls();
    }

    private void CameraControls()
    {
        if (cameraTurnHold)
        {
            if (Input.GetKeyDown(turnCamKey))
            {
                turnCam();
            }

            if (Input.GetKeyUp(turnCamKey))
            {
                turnCam();
            }
        }

        else
        {
            if (Input.GetKeyDown(turnCamKey))
            {
                turnCam();
            }
        }
    }

    private void turnCam()
    {
        if (mainCam.Priority == 1)
        {
            mainCam.Priority = 2;
            reverseCam.Priority = 1;
        }
        else
        {
            mainCam.Priority = 1;
            reverseCam.Priority = 2;
        }
    }

    private void turnCamCinematic()
    {
        if (cinematicCamTurned)
        {
            cinematicCamTurned = false;
            cinematicCamTurn = false;
        }
        else
        {
            cinematicCamTurned = true;
            cinematicCamTurn = false;
        }
    }
}

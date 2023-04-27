using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraExtras : MonoBehaviour
{
    //Camera controls
    [Header("Camera controls")]
    [SerializeField] private KeyCode turnCamKey = KeyCode.LeftControl;
    [SerializeField] private bool cameraTurnHold = true;

    private CinemachineVirtualCamera vCam;
    private CinemachineTransposer transposer;
    private Animator animator;
    private bool cinematicCamTurned = false;
    private bool cinematicCamTurn = false;
    public bool CinematicCamTurn { get { return cinematicCamTurn; } set { cinematicCamTurn = value; } }

    private void Start()
    {
        animator = GetComponent<Animator>();

        vCam = GetComponent<CinemachineVirtualCamera>();
        transposer = vCam.GetCinemachineComponent<CinemachineTransposer>();
    }

    private void Update()
    {
        CameraControls();

        if (cinematicCamTurn)
        {
            turnCamCinematic();
            animator.SetBool("Spin", cinematicCamTurned);
        }
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
        transposer.m_FollowOffset = new Vector3(transposer.m_FollowOffset.x, transposer.m_FollowOffset.y, -transposer.m_FollowOffset.z);
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

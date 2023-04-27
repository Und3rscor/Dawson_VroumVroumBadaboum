using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraExtras : MonoBehaviour
{
    //Camera controls
    [Header("Camera controls")]
    [SerializeField] private KeyCode turnCamKey = KeyCode.LeftControl;

    private CinemachineVirtualCamera vCam;
    private CinemachineTransposer transposer;
    private bool camTurned = false;
    public bool CamTurned { get { return camTurned; } set {  camTurned = value; } }

    private void Start()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        transposer = vCam.GetCinemachineComponent<CinemachineTransposer>();
    }

    private void Update()
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

    private void turnCam()
    {
        if (camTurned)
        {
            transposer.m_FollowOffset = new Vector3(0, 1.57f, -6);
            camTurned = false;
        }
        else
        {
            transposer.m_FollowOffset = new Vector3(0, 1.57f, 6);
            camTurned = true;
        }
    }
}

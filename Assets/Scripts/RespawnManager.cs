using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [Tooltip("How long does it take in seconds for the car to respawn")]
    public int respawnDelay;

    //Checkpoints
    private GameObject[] checkpoints;

    public int NextCheckpoint { get { return nextCheckpoint; } set { nextCheckpoint = value; } }
    private int nextCheckpoint;

    public int CurrentCheckpoint { get { return currentCheckpoint; } set { currentCheckpoint = value; } }
    private int currentCheckpoint;

    private UI ui;

    private void Start()
    {
        nextCheckpoint = 1;
        currentCheckpoint = 0;

        ui = GetComponentInChildren<UI>();
    }

    public void UpdateCheckpointList(GameObject[] managerCheckpoints)
    {
        checkpoints = managerCheckpoints;
    }

    public void UpdateLastCheckpointPassed()
    {
        //Updates the current checkpoint
        currentCheckpoint = nextCheckpoint;

        //Updates the next checkpoint
        nextCheckpoint = (nextCheckpoint + 1) % checkpoints.Length;

        ui.Checkpoint();
    }

    public void Respawn()
    {
        //Move the car to the lastCheckpointPassed
        this.gameObject.transform.position = checkpoints[currentCheckpoint].gameObject.transform.position;
        this.gameObject.transform.rotation = checkpoints[currentCheckpoint].gameObject.transform.rotation;

        //Tells the car to reenable itself
        GetComponent<ArcadeVehicleController>().CarRespawn();
    }
}

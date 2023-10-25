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
        //Creates a new checkpoint rotation based on the local rotation of the respawn point. Makes it easier to use them
        Quaternion spawnRotation = Quaternion.Euler(checkpoints[currentCheckpoint].gameObject.transform.localRotation.eulerAngles);

        if (RaceManager.Instance.test)
        {
            //Move the car to the nextCheckpoint
            this.gameObject.transform.position = checkpoints[nextCheckpoint].gameObject.transform.position;
            this.gameObject.transform.rotation = spawnRotation;
        }
        else
        {

        }

        //Tells the car to reenable itself
        GetComponent<ArcadeVehicleController>().CarRespawn();
    }
}

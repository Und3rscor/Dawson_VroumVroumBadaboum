using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [Tooltip("How long does it take in seconds for the car to respawn")]
    public int respawnDelay;

    //Checkpoints
    private Checkpoint[] checkpoints;

    public int NextCheckpoint { get { return nextCheckpoint; } set { nextCheckpoint = value; } }
    private int nextCheckpoint;

    public bool OverrideRespawnPoint { get { return overrideRespawnPoint; } set { overrideRespawnPoint = value; } }
    private bool overrideRespawnPoint = false;

    private UI ui;

    private void Start()
    {
        nextCheckpoint = 0;

        ui = GetComponentInChildren<UI>();
    }

    public void UpdateCheckpointList(Checkpoint[] managerCheckpoints)
    {
        checkpoints = managerCheckpoints;
    }

    public void UpdateNextCheckpoint(bool giveScore)
    {
        //Updates the next checkpoint
        nextCheckpoint++;

        if (giveScore)
        {
            ui.Checkpoint();
        }
    }

    public void Respawn()
    {
        GameObject respawnPoint = FetchRespawnPoint();

        //Creates a new checkpoint rotation based on the local rotation of the respawn point. Makes it easier to use them
        Quaternion spawnRotation = Quaternion.Euler(respawnPoint.transform.rotation.eulerAngles);

        //Move the car to the respawnPoint
        this.gameObject.transform.position = respawnPoint.transform.position + (Vector3.up * 10);
        this.gameObject.transform.rotation = spawnRotation;

        //Tells the car to reenable itself
        GetComponent<ArcadeVehicleController>().CarRespawn();

        if (overrideRespawnPoint)
        {
            overrideRespawnPoint = false;
        }
    }

    //If killed by out of bounds, set the respawnpoint to the previous checkpoint, otherwise set it to the blue zone
    private GameObject FetchRespawnPoint()
    {
        if (overrideRespawnPoint)
        {
            if (nextCheckpoint == 0)
            {
                //Sets the respawn point to the current checkpoint if it's the starting line
                return checkpoints[nextCheckpoint].gameObject;
            }
            else
            {
                //Sets the respawn point to the previous checkpoint
                return checkpoints[nextCheckpoint - 1].gameObject;
            }
        }
        else
        {
            //Fetches the respawnPoint from the raceManager
            return RaceManager.Instance.RespawnPoint;
        }
    }
}

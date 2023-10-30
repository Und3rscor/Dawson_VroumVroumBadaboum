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

    private UI ui;

    private void Start()
    {
        nextCheckpoint = 0;

        ui = GetComponentInChildren<UI>();
    }

    public void UpdateCheckpointList(GameObject[] managerCheckpoints)
    {
        checkpoints = managerCheckpoints;
    }

    public void UpdateNextCheckpoint()
    {
        //Updates the next checkpoint
        nextCheckpoint++;

        ui.Checkpoint();
    }

    public void Respawn()
    {
        //Fetches the respawnPoint from the raceManager
        GameObject respawnPoint = RaceManager.Instance.RespawnPoint;

        //Creates a new checkpoint rotation based on the local rotation of the respawn point. Makes it easier to use them
        Quaternion spawnRotation = Quaternion.Euler(respawnPoint.transform.localRotation.eulerAngles);

        //Move the car to the nextCheckpoint
        this.gameObject.transform.position = respawnPoint.transform.position;
        this.gameObject.transform.rotation = spawnRotation;

        //Tells the car to reenable itself
        GetComponent<ArcadeVehicleController>().CarRespawn();
    }
}

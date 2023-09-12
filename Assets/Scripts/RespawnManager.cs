using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [Tooltip("How long does it take in seconds for the car to respawn")]
    public float respawnDelay;
    private Vector3 lastCheckpointPassed;

    void Start()
    {
        lastCheckpointPassed = transform.position;
    }

    public void UpdateLastCheckpointPassed(Vector3 lastCheckpoint)
    {
        lastCheckpointPassed = lastCheckpoint;
    }

    public void Respawn()
    {
        //Move the car to the lastCheckpointPassed
        this.gameObject.transform.position = lastCheckpointPassed;

        //Tells the car to reenable itself
        GetComponent<ArcadeVehicleController>().CarRespawn();
    }
}

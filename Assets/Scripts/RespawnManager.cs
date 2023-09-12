using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [Tooltip("How long does it take in seconds for the car to respawn")]
    public int respawnDelay;
    private Vector3 lastCheckpointPassedPosition;
    private Quaternion lastCheckpointPassedRotation;

    void Start()
    {
        lastCheckpointPassedPosition = transform.position;
    }

    public void UpdateLastCheckpointPassed(Vector3 lastCheckpointPosition, Quaternion lastCheckpointRotation)
    {
        lastCheckpointPassedPosition = lastCheckpointPosition;
        lastCheckpointPassedRotation = lastCheckpointRotation;
    }

    public void Respawn()
    {
        //Move the car to the lastCheckpointPassed
        this.gameObject.transform.position = lastCheckpointPassedPosition;
        this.gameObject.transform.rotation = lastCheckpointPassedRotation;

        //Tells the car to reenable itself
        GetComponent<ArcadeVehicleController>().CarRespawn();
    }
}

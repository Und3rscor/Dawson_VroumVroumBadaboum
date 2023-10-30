using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueZone : MonoBehaviour
{
    void Update()
    {
        //Move to the first place player's position
        if (RaceManager.Instance != null && RaceManager.Instance.FirstPlacePlayer != null)
        {
            //Follows first place player
            transform.position = RaceManager.Instance.FirstPlacePlayer.transform.position;

            //Sets the rotation accordingly
            Quaternion newRotation = transform.rotation; // Get the current rotation
            newRotation.eulerAngles = new Vector3(newRotation.eulerAngles.x, RaceManager.Instance.FirstPlacePlayer.transform.rotation.eulerAngles.y, newRotation.eulerAngles.z);
            transform.rotation = newRotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //When a player enters the blue zone, they stop taking damage
        if (other.tag == "MainPlayer")
        {
            other.GetComponentInParent<ArcadeVehicleController>().EnterBlueZone();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //When a player leaves the blue zone, they start taking damage
        if (other.tag == "MainPlayer")
        {
            other.GetComponentInParent<ArcadeVehicleController>().LeaveBlueZone();
        }
    }
}

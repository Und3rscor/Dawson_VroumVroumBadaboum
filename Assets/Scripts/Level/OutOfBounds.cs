using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "MainPlayer")
        {
            ArcadeVehicleController player = other.gameObject.GetComponent<ArcadeVehicleController>();

            if (player != RaceManager.Instance.FirstPlacePlayer)
            {
                //Move respawn point
                player.gameObject.GetComponent<RespawnManager>().OverrideRespawnPoint = true;
            }
            //Blows the player up
            player.BlowUp(null);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "MainPlayer")
        {
            other.GetComponentInParent<ArcadeVehicleController>().BlowUp();
        }

        if (other.transform.tag == "MainBot" || other.transform.tag == "Bot")
        {
            other.GetComponentInParent<BotHP>().Die();
        }
    }
}

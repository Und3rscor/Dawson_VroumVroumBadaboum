using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "MainPlayer" && other.gameObject.GetComponentInChildren<UI>() != null)
        {
            UI ui = other.gameObject.GetComponentInChildren<UI>();

            ui.lapAvailable = true;

            other.GetComponent<RespawnManager>().UpdateLastCheckpointPassed(this.transform.position + Vector3.up, this.transform.rotation);
        }
    }
}

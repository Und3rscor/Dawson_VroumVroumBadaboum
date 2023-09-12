using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "MainPlayer" && other.gameObject.GetComponentInChildren<UI>() != null)
        {
            UI ui = other.gameObject.GetComponentInChildren<UI>();

            if (ui.lapAvailable)
            {
                ui.Lap();
                other.GetComponentInChildren<MachineGun>().RefillAmmo();
                other.GetComponentInParent<ArcadeVehicleController>().RefillNos();
                ui.lapAvailable = false;

                other.GetComponent<RespawnManager>().UpdateLastCheckpointPassed(this.transform.position + Vector3.up, this.transform.rotation);

                if (GameManager.Instance.Alive <= 1)
                {
                    //GameManager.Instance.GameOverDelay();
                }
            }
        }
    }
}

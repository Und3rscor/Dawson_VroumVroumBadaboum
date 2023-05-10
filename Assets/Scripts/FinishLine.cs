using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "MainPlayer" && GameManager.Instance.lapAvailable)
        {
            GameManager.Instance.Lap();
            other.GetComponentInChildren<MachineGun>().RefillAmmo();
            other.GetComponentInParent<ArcadeVehicleController>().RefillNos();
            GameManager.Instance.lapAvailable = false;

            if (GameManager.Instance.Alive <= 1)
            {
                GameManager.Instance.GameOverDelay();
            }
        }
    }
}

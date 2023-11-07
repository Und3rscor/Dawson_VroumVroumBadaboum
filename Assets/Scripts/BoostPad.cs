using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86;

public class BoostPad : MonoBehaviour
{
    [SerializeField] private int boostPadID;

    public int BoostPadLevel { get { return boostPadLevel; } set { boostPadLevel = value; } }
    private int boostPadLevel = 0;

    private MeshRenderer boostPadRenderer;

    private void Start()
    {
        boostPadRenderer = GetComponentInChildren<MeshRenderer>();

        if (boostPadRenderer != null)
        {
            SetBoostPadColor(Color.black);
        }
    }

    private void UpgradeBoostPad(BoostPad boostpad)
    {
        boostpad.BoostPadLevel++;

        if (boostpad.boostPadLevel == 1)
        {
            boostpad.SetBoostPadColor(Color.green);
        }
        else if (boostpad.boostPadLevel == 2)
        {
            boostpad.SetBoostPadColor(Color.blue);
        }
        else
        {
            Debug.Log("wtf?");
        }
    }

    public void SetBoostPadColor(Color color)
    {
        Material[] materials = boostPadRenderer.materials; // Get a reference to the materials array

        Material mat = materials[0]; // Get a reference to the current material

        // Modify the color property of the material
        mat.color = color;

        // Update the materials on the MeshRenderer
        boostPadRenderer.materials = materials;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ArcadeVehicleController>() != null)
        {
            //Grabs the AVC
            ArcadeVehicleController avc = other.GetComponent<ArcadeVehicleController>();

            //Boosts the player
            if (boostPadID == 0)
            {
                avc.RiBy.AddForce(this.gameObject.transform.forward * RaceManager.Instance.BoostForce / 2, ForceMode.Impulse);
            }
            else if (boostPadLevel == 1)
            {
                avc.RiBy.AddForce(this.gameObject.transform.forward * RaceManager.Instance.BoostForce, ForceMode.Impulse);
            }
            else if (boostPadLevel == 2)
            {
                avc.carBody.AddTorque(this.gameObject.transform.forward * RaceManager.Instance.BoostForce * 2, ForceMode.Impulse);
            }

            //If it's the first player, upgrade boostpad
            if (avc.gameObject.GetComponent<RespawnManager>() == RaceManager.Instance.FirstPlacePlayer)
            {
                //Upgrade this pad
                UpgradeBoostPad(this);

                //Upgrade previous one
                if (boostPadID >= 1)
                {
                    UpgradeBoostPad(RaceManager.Instance.BoostPads[boostPadID - 1].GetComponentInChildren<BoostPad>());
                }
            }            
        }
    }
}

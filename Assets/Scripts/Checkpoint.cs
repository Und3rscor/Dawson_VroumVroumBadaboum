using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Tooltip("If it's the starting line, leave at 0, otherwise, increment them in order that the player would reach them")]
    [SerializeField] private int checkpointID;

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

    private void UpgradeBoostPad(Checkpoint checkpoint)
    {
        checkpoint.BoostPadLevel++;

        if (checkpoint.boostPadLevel == 1)
        {
            checkpoint.SetBoostPadColor(Color.green);
        }
        else if (checkpoint.boostPadLevel == 2)
        {
            checkpoint.SetBoostPadColor(Color.blue);
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
        if (other.gameObject.GetComponent<RespawnManager>() != null)
        {
            //Grabs the respawn manager script for later use
            RespawnManager rm = other.gameObject.GetComponentInChildren<RespawnManager>();

            //Grabs the AVC
            ArcadeVehicleController avc = rm.GetComponent<ArcadeVehicleController>();

            //Only trigger the checkpoint if it was your goal checkpoint before you interact with it
            if (rm.NextCheckpoint == checkpointID)
            {
                //Updates the respawn manager to know what is it's next target checkpoint
                other.GetComponent<RespawnManager>().UpdateNextCheckpoint();

                //Refills the car's nos
                other.GetComponentInParent<ArcadeVehicleController>().RefillNos();

                //If it's the first player, upgrade boostpad
                if (rm == RaceManager.Instance.FirstPlacePlayer)
                {
                    //Upgrade this pad
                    UpgradeBoostPad(this);

                    //Upgrade previous one
                    if (checkpointID >= 1)
                    {
                        UpgradeBoostPad(RaceManager.Instance.Checkpoints[checkpointID - 1].GetComponentInChildren<Checkpoint>());
                    }
                }
                
                //Otherwise, boost player
                else
                {
                    if (boostPadLevel == 1)
                    {
                        avc.RiBy.AddForce(this.gameObject.transform.forward * RaceManager.Instance.BoostForce, ForceMode.Impulse);
                    }
                    else if (boostPadLevel == 2)
                    {
                        avc.carBody.AddTorque(Vector3.up * RaceManager.Instance.BoostForce * 2, ForceMode.Impulse);
                    }
                }

                if (this.gameObject.tag == "Finishline")
                {
                    UI ui = other.gameObject.GetComponentInChildren<UI>();

                    //Launch Scoreboard
                    RaceManager.Instance.FinishScene();
                }
            }
        }
    }
}

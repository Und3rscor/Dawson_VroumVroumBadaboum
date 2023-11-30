using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Tooltip("If it's the starting line, leave at 0, otherwise, increment them in order that the player would reach them")]
    [SerializeField] private int checkpointID;
    [SerializeField] private bool isIngameCheckpoint;

    public int CheckpointID { get { return checkpointID; } }

    private void Start()
    {
        //If it's not an ingame checkpoint, turn off the renderer
        if (!isIngameCheckpoint)
        {
            this.transform.parent.gameObject.GetComponentInChildren<Animator>().gameObject.SetActive(false);
        }
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
                //If it's an ingame checkpoint, it will give score otherwise, it won't
                rm.UpdateNextCheckpoint(isIngameCheckpoint);

                //Refills the car's nos only if it's an ingame checkpoint and not a manager checkpoint
                if (isIngameCheckpoint)
                {
                    avc.RefillNos();
                }
                
                if (this.gameObject.tag == "Finishline")
                {
                    //Launch Scoreboard
                    RaceManager.Instance.FinishScene();
                }
            }
        }
    }
}

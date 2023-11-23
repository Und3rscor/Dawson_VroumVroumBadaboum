using PowerslideKartPhysics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    [Tooltip("Set the array amount to the max value of checkpointIDs used plus 1")]
    [SerializeField] private Checkpoint[] checkpoints;

    public Checkpoint[] Checkpoints { get { return checkpoints; } }

    // Start is called before the first frame update
    void Start()
    {
        FindAllCheckPoints(this.transform);
    }

    /// <summary>
    /// Takes care of assigning all the checkpoints in the scene based on their respective IDs
    /// </summary>
    private void FindAllCheckPoints(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Checkpoint checkpoint = child.GetComponentInChildren<Checkpoint>();

            // Check if the checkpoint is not null and checkpointID is a valid index
            if (checkpoint != null && checkpoint.CheckpointID >= 0 && checkpoint.CheckpointID < checkpoints.Length)
            {
                checkpoints[checkpoint.CheckpointID] = checkpoint;
            }
            else
            {
                Debug.LogError($"Invalid Checkpoint or CheckpointID: {checkpoint?.name} with ID {checkpoint?.CheckpointID}. Check the CheckpointID values.");
            }
        }
    }
}

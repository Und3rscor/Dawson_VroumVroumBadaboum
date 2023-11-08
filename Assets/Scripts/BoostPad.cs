using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86;

public class BoostPad : MonoBehaviour
{
    [SerializeField] private int boostPadID;
    [SerializeField] private int materialToChangeColorInTheRenderer;

    public int BoostPadLevel { get { return boostPadLevel; } set { boostPadLevel = value; } }
    private int boostPadLevel = 0;

    private List<MeshRenderer> boostPadRendererList;

    private void Start()
    {
        boostPadRendererList = new List<MeshRenderer>();

        FindMeshRenderers(this.transform);

        if (boostPadRendererList.Count > 0)
        {
            SetColor(Color.white, materialToChangeColorInTheRenderer);
        }
    }

    private void FindMeshRenderers(Transform parentTransform)
    {
        // Loop through each child of the parentTransform
        foreach (Transform child in parentTransform)
        {
            // Check if the child has a MeshRenderer component
            MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();

            if (meshRenderer != null)
            {
                boostPadRendererList.Add(meshRenderer);
            }

            // Recursively search the child's children
            FindMeshRenderers(child);
        }
    }

    private void SetBoostPadLevel(BoostPad boostpad, int level)
    {
        boostpad.BoostPadLevel = level;

        if (boostpad.boostPadLevel == 1)
        {
            boostpad.SetColor(Color.green, materialToChangeColorInTheRenderer);
        }
        else if (boostpad.boostPadLevel == 2)
        {
            boostpad.SetColor(Color.blue, materialToChangeColorInTheRenderer);
        }
        else
        {
            Debug.Log("wtf?");
        }
    }

    public void SetColor(Color color, int materialToChangeColor)
    {
        foreach (MeshRenderer meshRenderer in boostPadRendererList)
        {
            Material[] materials = meshRenderer.materials; //Get a reference to the materials array

            Material mat = materials[materialToChangeColor]; //Get a reference to the material to change color

            mat.color = color; //Change it's color

            //Assign the modified materials array back to the meshRenderer
            meshRenderer.materials = materials;
        }
    }

    private void BoostPlayer(Rigidbody rb)
    {
        //Boosts the player
        if (boostPadID == 0)
        {
            rb.AddForce(rb.transform.up * RaceManager.Instance.Lvl0boostForce, ForceMode.Impulse);
        }
        else if (boostPadLevel == 1)
        {
            rb.AddForce(rb.transform.up * RaceManager.Instance.Lvl1boostForce, ForceMode.Impulse);
        }
        else if (boostPadLevel == 2)
        {
            rb.AddForce(rb.transform.up * RaceManager.Instance.Lvl2boostForce, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ArcadeVehicleController>() != null)
        {
            //Grabs the AVC
            ArcadeVehicleController avc = other.GetComponent<ArcadeVehicleController>();

            BoostPlayer(avc.RiBy);

            //If it's the first player, upgrade boostpad
            if (avc.gameObject.GetComponent<RespawnManager>() == RaceManager.Instance.FirstPlacePlayer)
            {
                //Upgrade this pad
                SetBoostPadLevel(this, 1);

                //Upgrade previous ones
                if (boostPadID >= 1)
                {
                    //Sets the first previous
                    SetBoostPadLevel(RaceManager.Instance.BoostPads[boostPadID - 1].GetComponentInChildren<BoostPad>(), 2);
                }
            }            
        }
    }

    
}

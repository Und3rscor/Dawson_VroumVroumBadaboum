using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerColorSetup : MonoBehaviour
{
    [SerializeField] private GameObject VFX;

    private List<MeshRenderer> meshRendererList;
    private List<VisualEffect> vfxList;

    private void Start()
    {
        meshRendererList = new List<MeshRenderer>();            //Initialize meshRendererList to store the meshRenderers found in the function below
        vfxList = new List<VisualEffect>();                     //Initialize Visual Effect list

        FindMeshRenderers(this.gameObject.transform);           //Finds all meshRendrers to toggle on and off during death sequences
        FindVFX(VFX.transform);                                 //Finds all Visual Effects in the vfx object
    }

    // This function will recursively find all MeshRenderers in the children of the specified transform
    private void FindMeshRenderers(Transform parentTransform)
    {
        // Loop through each child of the parentTransform
        foreach (Transform child in parentTransform)
        {
            // Check if the child has a MeshRenderer component
            MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();

            if (meshRenderer != null)
            {
                meshRendererList.Add(meshRenderer);
            }

            // Recursively search the child's children
            FindMeshRenderers(child);
        }

        //SetupColor(primaryColor, secondaryColor);
    }



    private void FindVFX(Transform parentTransform)
    {
        // Loop through each child of the parentTransform
        foreach (Transform child in parentTransform)
        {
            // Check if the child has a Visual Effect component
            VisualEffect vfx = child.GetComponent<VisualEffect>();

            if (vfx != null)
            {
                vfxList.Add(vfx);
            }

            // Recursively search the child's children
            FindVFX(child);
        }

        //SetupVFXColor(primaryColor, secondaryColor);
    }



    //Sets the color of the car body and the gradient of the trail vfx
    public void SetupColor(Material _primaryColor, Color _secondaryColor)
    {
        foreach (MeshRenderer meshRenderer in meshRendererList)
        {
            Material[] materials = meshRenderer.materials; // Get a reference to the materials array

            for (int i = 0; i < materials.Length; i++)
            {
                Material mat = materials[i]; // Get a reference to the current material

                // Changes all the base mat of the car to the chosen mat
                if (mat.name == "Synthwave_base (Instance)")
                {
                    materials[i] = _primaryColor; // Modify the material in the array
                }

                // Changes all the base neon mat of the car to the chosen neon mat
                if (mat.name == "Synthwave_neon_base (Instance)")
                {
                    materials[i].color = _secondaryColor; // Modify the material in the array
                }
            }

            // Assign the modified materials array back to the meshRenderer
            meshRenderer.materials = materials;
        }

        Debug.Log("ForE");

        //Creates a new gradient for the vfx
        var gradient = new Gradient();

        // Blend color from red at 0% to blue at 100%
        var colors = new GradientColorKey[2];
        colors[0] = new GradientColorKey(_primaryColor.color, 1.0f);
        colors[1] = new GradientColorKey(_secondaryColor, 0.0f);

        // Blend alpha from opaque at 0% to transparent at 100%
        var alphas = new GradientAlphaKey[2];
        alphas[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphas[1] = new GradientAlphaKey(0.0f, 1.0f);

        gradient.SetKeys(colors, alphas);

        foreach (VisualEffect vfx in vfxList)
        {
            //Sets the value of the gradient created above
            vfx.SetGradient("SparksGradient", gradient);
        }

        //Removed for the time being
        /*
        if (randomColors)
        {
            primaryColor = _primaryColor.color;
            secondaryColor = _secondaryColor;
        }
        */
    }

    public void RespawnToggle(bool active)
    {
        //Toggles meshes so they car is invisible while dead
        foreach (MeshRenderer meshRenderers in meshRendererList)
        {
            meshRenderers.enabled = active;
        }

        //Toggles visual effects
        foreach (VisualEffect visualEffect in vfxList)
        {
            visualEffect.enabled = active;
        }
    }
}

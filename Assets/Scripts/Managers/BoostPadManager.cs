using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoostPadManager : MonoBehaviour
{
    [Tooltip("Set the array amount to the max value of boostPadID used plus 1")]
    [SerializeField] private BoostPad[] boostPads;

    //BoostPad force per level
    [SerializeField] private float lvl0boostForce;
    [SerializeField] private float lvl1boostForce;
    [SerializeField] private float lvl2boostForce;

    //Relays
    public float Lvl0boostForce { get { return lvl0boostForce; } }
    public float Lvl1boostForce { get { return lvl1boostForce; } }
    public float Lvl2boostForce { get { return lvl2boostForce; } }
    public BoostPad[] BoostPads { get { return boostPads; } }

    //BoostPadManager Instance
    public static BoostPadManager Instance { get { return instance; } }
    private static BoostPadManager instance;

    private void Awake()
    {
        //Instance stuff
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        FindAllBoostPads();
    }

    /// <summary>
    /// Takes care of assigning all the boostpads in the scene based on their respective IDs
    /// </summary>
    private void FindAllBoostPads()
    {
        foreach (Transform child in this.transform)
        {
            BoostPad boostpad = child.GetComponent<BoostPad>();

            // Check if the boostpad is not null and BoostPadID is a valid index
            if (boostpad != null && boostpad.BoostPadID >= 0 && boostpad.BoostPadID < boostPads.Length)
            {
                boostPads[boostpad.BoostPadID] = boostpad;
            }
            else
            {
                Debug.LogError($"Invalid BoostPad or BoostPadID: {boostpad?.name} with ID {boostpad?.BoostPadID}. Check the BoostPadID values.");
            }
        }
    }
}

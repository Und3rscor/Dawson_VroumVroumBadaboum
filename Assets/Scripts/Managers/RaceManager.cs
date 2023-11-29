using Cinemachine;
using Michsky.UI.ModernUIPack;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class RaceManager : MonoBehaviour
{
    [SerializeField] private GameObject respawnPoint;

    private CheckpointManager checkpointManager;

    //BlueZone
    [SerializeField] private int blueZoneDps;
    public int BlueZoneDps { get { return blueZoneDps;} }

    //Bump
    public float BumpForce { get { return bumpForce; } }
    public float BumpTorque { get { return bumpTorque; } }
    [SerializeField] private float bumpForce;
    [SerializeField] private float bumpTorque;

    //Racemanager Instance
    private static RaceManager instance;

    public static RaceManager Instance { get { return instance; } }

    //Relays
    private UI ui;
    private PlayerConfig playerConfig;

    public bool GameOverBool { get { return gameOver; } }
    private bool gameOver = false;

    public RespawnManager FirstPlacePlayer { get { return firstPlacePlayer; } }
    public GameObject RespawnPoint { get { return respawnPoint; } }

    //Camera layers
    private int playerID = 1;

    //Private variables
    private bool paused = false;
    private List<RespawnManager> playerList;
    private RespawnManager firstPlacePlayer = null;
    private float firstPlacePlayerDistanceToFinish = float.MaxValue;
    private int firstPlacePlayerNextCheckpoint = 0;

    private void Start()
    {
        Pause();

        checkpointManager = FindObjectOfType<CheckpointManager>();

        //Initialize playerList
        playerList = new List<RespawnManager>();

        //FindUI();
    }

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

    private void Update()
    {
        // Find the player in first place
        foreach (RespawnManager player in playerList)
        {
            CheckDistanceToFinishLine(player);
        }
    }

    //Checks the distance between a given player and the finishline
    private void CheckDistanceToFinishLine(RespawnManager player)
    {
        //Checks if the player is competing for the same checkpoint as the first player
        if (player.NextCheckpoint >= firstPlacePlayerNextCheckpoint)
        {
            //Calculates distance to said checkpoint
            float distance = Vector3.Distance(player.transform.position, checkpointManager.Checkpoints[player.NextCheckpoint].transform.position);

            //Then checks if the distance of the player is smaller than the distance of the player in first place
            //If it is smaller, assigns that player as the new first place player
            //If the player if the first place player, assigns it's current distance
            if (distance < firstPlacePlayerDistanceToFinish || player == firstPlacePlayer)
            {
                firstPlacePlayer = player;
                firstPlacePlayerDistanceToFinish = distance;
                firstPlacePlayerNextCheckpoint = player.NextCheckpoint;
            }
        }
    }

    private void Pause()
    {
        Time.timeScale = 0.0f;
    }

    private void Resume()
    {
        Time.timeScale = 1.0f;
    }

    public void FinishScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    //Asked by CameraExtras.cs to setup it's camera
    public void PlayerSetup(GameObject obj, PlayerConfig pc)
    {
        Resume();

        //Grabs the player's AVC
        ArcadeVehicleController player = obj.GetComponentInParent<ArcadeVehicleController>();
        Debug.Log("Avc");

        //Grabs the player object
        GameObject playerObj = player.gameObject;
        Debug.Log("obj");

        //Grabs the playerConfigs
        playerConfig = pc;
        Debug.Log("pc");

        //Changes the car color
        player.PlayerColor.SetupColor(pc.color, Color.black);
        Debug.Log("clr");

        //Grabs the respawnManager
        RespawnManager rm = playerObj.GetComponent<RespawnManager>();
        Debug.Log("rm");

        //Changes the name of the player object for easier access in debugging
        playerObj.name = "Player " + playerID;

        //Adds the player to the list of players
        playerList.Add(rm);

        //Changes the player's checkpoint list to the list provided
        player.RespawnManager.UpdateCheckpointList(checkpointManager.Checkpoints);

        //Gives the player car a random color
        //Feature removed for the time being
        /*
        if (playerColor.RandomColor)
        {
            playerColor.SetupColor(RandomColor(), RandomColor());
        }
        */

        //Sets the camera LayerMask between "P1 Cam" to "P4 Cam" depending on the player ID
        obj.layer = LayerMask.NameToLayer("P" + playerID + " Cam");

        //Then sets the same layer for it's children
        foreach (Transform child in obj.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("P" + playerID + " Cam");
        }

        Camera camBrain = obj.GetComponentInChildren<CameraExtras>().GetComponent<Camera>();

        //Sets the camera CullingMask to remove the other cameras
        ChangeCameraCulling(camBrain);

        //Removes the audio listener from the other cars
        if (playerID > 1)
        {
            obj.GetComponent<AudioListener>().enabled = false;
        }

        //This is just so that there are no duplicate player IDs
        playerID++;
    }

    private Color RandomColor()
    {
        Color[] colors = { Color.black, Color.blue, Color.cyan, Color.gray, Color.green, Color.magenta, Color.red, Color.white, Color.yellow };
        int randomIndex = Random.Range(0, colors.Length);

        return colors[randomIndex];
    }

    private void ChangeCameraCulling(Camera brain)
    {
        // Retrieve the current culling mask of the camera
        int currentCullingMask = brain.cullingMask;

        // Checks which layers to remove
        int[] layersToRemove = CullingLayersToRemoveFromMask();

        // Remove the layers from the culling mask using bitwise operations
        foreach (int layer in layersToRemove)
        {
            currentCullingMask &= ~(1 << layer);
        }
        int newCullingMask = currentCullingMask;

        // Set the Camera's culling mask to the modified mask
        brain.cullingMask = newCullingMask;
    }

    private int[] CullingLayersToRemoveFromMask()
    {
        int[] cullings = new int[3];
        int index = 0;

        //Removes all camera layers one by one except for the layer the object is on
        for (int i = 1; i <= 4; i++)
        {
            if (playerID != i)
            {
                cullings[index++] = LayerMask.NameToLayer("P" + i + " Cam");
            }
        }

        return cullings;
    }
}

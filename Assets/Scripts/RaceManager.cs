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
    [SerializeField] private Transform[] spawnpoints;

    [Tooltip("Put the starting line at 0 then all other checkpoints in the order the players will encounter them and the finishline last")]
    [SerializeField] private GameObject[] checkpoints;

    //BlueZone
    [SerializeField] private int blueZoneDps;
    public int BlueZoneDps { get { return blueZoneDps;} }

    //Bump
    public float BumpForce { get { return bumpForce; } }
    public float BumpTorque { get { return bumpTorque; } }
    [SerializeField] private float bumpForce;
    [SerializeField] private float bumpTorque;

    //Gamemanager Instance
    private static RaceManager instance;

    public static RaceManager Instance { get { return instance; } }

    //Relays
    private UI ui;

    public int Alive { get { return alive; } set { alive = value; } }
    private int alive;

    public int TotalAlive { get { return totalAlive; } }
    private int totalAlive;

    public bool GameOverBool { get { return gameOver; } }
    private bool gameOver = false;

    public GameObject FirstPlacePlayer { get { return firstPlacePlayer; } }

    //Camera layers
    private int playerID = 1;

    //Private variables
    private bool paused = false;
    private List<GameObject> playerList;
    private GameObject firstPlacePlayer = null;
    private float firstPlacePlayerDistanceToFinish = float.MaxValue;

    private void Start()
    {
        Pause();

        Setup();

        //Initialize playerList
        playerList = new List<GameObject>();

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
        if (alive <= 0 && totalAlive >= 1)
        {
            FinishScene();
        }

        // Find the player in first place
        foreach (GameObject player in playerList)
        {
            CheckDistanceToFinishLine(player);
        }
    }

    //Checks the distance between a given player and the finishline
    private void CheckDistanceToFinishLine(GameObject player)
    {
        //Calculates distance
        float distance = Vector3.Distance(player.transform.position, checkpoints[checkpoints.Length - 1].transform.position);

        //Then checks if the distance of the player is smaller than the distance of the player in first place
        //If it is smaller, assigns that player as the new first place player
        //If the player if the first place player, assigns it's current distance
        if (distance < firstPlacePlayerDistanceToFinish || player == firstPlacePlayer)
        {
            firstPlacePlayer = player;
            firstPlacePlayerDistanceToFinish = distance;
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

    public void Setup()
    {
        totalAlive = GameObject.FindGameObjectsWithTag("MainPlayer").Length + GameObject.FindGameObjectsWithTag("MainBot").Length;
        alive = totalAlive;
    }

    //Asked by CameraExtras.cs to setup it's camera
    public void PlayerSetup(GameObject obj, Camera camBrain)
    {
        Resume();

        //Updates the alive counter
        totalAlive++;
        alive++;

        if (spawnpoints != null)
        {
            //Grabs the player's AVC
            ArcadeVehicleController player = obj.GetComponentInParent<ArcadeVehicleController>();

            //Grabs the player object
            GameObject playerObj = player.gameObject;

            //Changes the name of the player object for easier access in debugging
            playerObj.name = "Player " + playerID;

            //Adds the player to the list of players
            playerList.Add(playerObj);

            //Sets the playerSpawnpoint to the spawnpoint it gets assigned
            Transform playerSpawnPoint = spawnpoints[playerID - 1];

            //Changes the position and rotation of the player to their spawn point when they spawn
            player.transform.position = playerSpawnPoint.position;
            player.transform.rotation = playerSpawnPoint.rotation;

            //Changes the player's checkpoint list to the list provided
            player.RespawnManager.UpdateCheckpointList(checkpoints);

            //Gives the player car a random color
            if (player.RandomColor)
            {
                player.SetupColor(RandomColor(), RandomColor());
            }
        }
        
        //Sets the camera LayerMask between "P1 Cam" to "P4 Cam" depending on the player ID
        obj.layer = LayerMask.NameToLayer("P" + playerID + " Cam");

        //Then sets the same layer for it's children
        foreach (Transform child in obj.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("P" + playerID + " Cam");
        }

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

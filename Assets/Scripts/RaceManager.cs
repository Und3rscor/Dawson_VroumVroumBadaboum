using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class RaceManager : MonoBehaviour
{
    [SerializeField] private Transform[] spawnpoints;

    [Tooltip("Put the finish line at 0 and all other checkpoints in the order the players will encounter them")]
    [SerializeField] private GameObject[] checkpoints;

    //Gamemanager Instance
    private static RaceManager instance;

    public static RaceManager Instance { get { return instance; } }

    //Fetches
    private UI ui;

    //Alive variable
    private int alive;

    public int Alive { get { return alive; } set { alive = value; } }

    //Total Alive variable
    private int totalAlive;
    public int TotalAlive { get { return totalAlive; } }

    //Camera layers
    private int playerID = 1;

    //Private variables
    public bool GameOverBool { get { return  gameOver; } }
    private bool gameOver = false;

    private bool paused = false;

    private void Start()
    {
        Pause();

        Setup();

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

            //Sets the playerSpawnpoint to the spawnpoint it gets assigned
            Transform playerSpawnPoint = spawnpoints[playerID - 1];

            //Changes the position and rotation of the player to their spawn point when they spawn
            player.transform.position = playerSpawnPoint.position;
            player.transform.rotation = playerSpawnPoint.rotation;

            //Changes the player's checkpoint list to the list provided
            player.RespawnManager.UpdateCheckpointList(checkpoints);
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

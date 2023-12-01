using Cinemachine;
using Michsky.UI.ModernUIPack;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class RaceManager : MonoBehaviour
{
    [SerializeField] private GameObject respawnPoint;

    private List<PlayerInput> playerInputs;

    private int playerID;

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

    //Private variables
    private List<RespawnManager> playerList;
    private RespawnManager firstPlacePlayer = null;
    private float firstPlacePlayerDistanceToFinish = float.MaxValue;
    private int firstPlacePlayerNextCheckpoint = 0;

    private void Start()
    {
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

        playerInputs = new List<PlayerInput>();
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
            if (player.NextCheckpoint < CheckpointManager.Instance.Checkpoints.Length)
            {
                Vector3 target = CheckpointManager.Instance.Checkpoints[player.NextCheckpoint].transform.position;
                float distance = Vector3.Distance(player.transform.position, target);
                
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
    }

    public void Pause()
    {
        Time.timeScale = 0.0f;

        InputToggle(false);
    }

    public void Resume()
    {
        Time.timeScale = 1.0f;

        InputToggle(true);
    }

    public void FinishScene(ArcadeVehicleController winningPlayer)
    {
        var playerConfigs = PlayerConfigManager.Instance.GetPlayerConfigs().ToArray();
        var players = playerList.ToArray();

        //Gives all ui scores to the player configs
        for (int i = 0; i < playerConfigs.Length; i++)
        {
            var player = players[i].GetComponent<ArcadeVehicleController>();

            Debug.Log(player.UI.Kills);
            playerConfigs[i].Score = player.UI.Score;
            playerConfigs[i].Kills = player.UI.Kills;
            playerConfigs[i].Name = player.gameObject.name;

            if (player == winningPlayer)
            {
                playerConfigs[i].FirstPlayer = true;
            }
            else
            {
                playerConfigs[i].FirstPlayer = false;
            }
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    //Asked by CameraExtras.cs to setup it's camera
    public void PlayerSetup(GameObject obj, PlayerConfig pc)
    {
        //Grabs the ArcadeVehicleController
        PlayerControllerRelay pCR = obj.GetComponent<PlayerControllerRelay>();

        //Grabs the colorer
        PlayerColorSetup colorer = obj.GetComponentInChildren<PlayerColorSetup>();

        //Grabs the playerInput
        PlayerInput input = pc.Input;

        //Adds the input to the list of inputs
        playerInputs.Add(input);

        //Sets the input
        pCR.SetPlayerInput(input);

        //Changes the default map
        input.SwitchCurrentActionMap("Player");

        //Player ID thing
        playerID = pc.PlayerIndex + 1;

        //Grabs the playerConfigs
        playerConfig = pc;

        //Changes the car color
        colorer.SetupColor(pc.Mat, Color.black);

        //Grabs the respawnManager
        RespawnManager rm = obj.GetComponent<RespawnManager>();

        //Changes the name of the player object for easier access in debugging
        obj.name = "Player " + playerID;

        //Adds the player to the list of players
        playerList.Add(rm);

        //Changes the player's checkpoint list to the list provided
        rm.UpdateCheckpointList(CheckpointManager.Instance.Checkpoints);

        //Gives the player car a random color
        //Feature removed for the time being
        /*
        if (playerColor.RandomColor)
        {
            playerColor.SetupColor(RandomColor(), RandomColor());
        }
        */

        //Sets the camera CullingMask to remove the other cameras
        Camera camBrain = obj.GetComponentInChildren<CameraExtras>().GetComponent<Camera>();
        ChangeCameraCulling(camBrain);

        //Sets the camera LayerMask between "P1 Cam" to "P4 Cam" depending on the player ID
        camBrain.gameObject.layer = LayerMask.NameToLayer("P" + playerID + " Cam");

        //Then sets the same layer for it's children
        foreach (Transform child in camBrain.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("P" + playerID + " Cam");
        }

        //Adds the camera to the player input
        input.camera = camBrain;

        //Removes the audio listener from the other cars
        if (pc.PlayerIndex >= 1)
        {
            obj.GetComponentInChildren<AudioListener>().enabled = false;
        }
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

    private void InputToggle(bool toggle)
    {
        foreach (PlayerInput input in playerInputs)
        {
            if (!toggle)
            {
                input.DeactivateInput();
            }
            else
            {
                input.ActivateInput();
            }
        }
    }
}

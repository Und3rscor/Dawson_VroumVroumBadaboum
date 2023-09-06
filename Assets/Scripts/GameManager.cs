using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviour
{
    //Gamemanager Instance
    private static GameManager instance;

    public static GameManager Instance { get { return instance; } }

    //Spawning stuff
    [SerializeField] private GameObject[] spawnpoints;

    //Find the UI Manager
    private UI ui;

    //Debug Keys
    [SerializeField] private KeyCode kill;

    //Keybinds
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
    [SerializeField] private KeyCode screenshotKey = KeyCode.F10;

    //Alive variable
    private int alive;

    public int Alive { get { return alive; } }

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
        Setup();

        FindUI();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        QuickDebug();

        if (Input.GetKeyDown(screenshotKey))
        {
            Screenshot();
        }

        if (Input.GetKeyDown(pauseKey))
        {
            if (paused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    private void QuickDebug()
    {
        //Simulates 1 kill
        if (Input.GetKeyDown(kill))
        {
            Kill();
        }
    }

    private void Screenshot()
    {
        ScreenCapture.CaptureScreenshot("screenshot.png");
        Debug.Log("A screenshot was taken!");
    }

    public void Kill()
    {
        alive--;
        //scoreBonus += 1000;
    }

    public void Restart()
    {
        //laps = 0;
        gameOver = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GameOverDelay()
    {
        Invoke("GameOver", 0.5f);
    }

    private void GameOver()
    {
        gameOver = true;

        if (ui != null)
        {
            ui.UIRedraw(ui.gameOverUI);
        }
    }

    private void Pause()
    {
        InputSystem.DisableAllEnabledActions();

        paused = true;

        if (ui != null)
        {
            ui.UIRedraw(ui.pauseUI);
        }
    }

    public void Resume()
    {
        InputSystem.ResumeHaptics();
        paused = false;

        if (ui != null)
        {
            ui.UIRedraw(ui.gameUI);
        }
    }

    public void Setup()
    {
        totalAlive = GameObject.FindGameObjectsWithTag("MainPlayer").Length + GameObject.FindGameObjectsWithTag("MainBot").Length;
        alive = totalAlive;
    }

    public void FindUI()
    {
        ui = FindObjectOfType<UI>();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    //Asked by CameraExtras.cs to setup it's camera
    public void CameraSetup(GameObject obj, Camera camBrain)
    {
        obj.GetComponentInParent<ArcadeVehicleController>().gameObject.transform.position = spawnpoints[playerID - 1].transform.position;

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

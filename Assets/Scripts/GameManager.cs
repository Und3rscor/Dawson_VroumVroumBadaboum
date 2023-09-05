using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Gamemanager Instance
    private static GameManager instance;

    public static GameManager Instance { get { return instance; } }

    //Find the UI Manager
    private UI ui;

    //Debug Keys
    [SerializeField] private KeyCode increaseLaps;
    [SerializeField] private KeyCode kill;

    //Keybinds
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
    [SerializeField] private KeyCode screenshotKey = KeyCode.F10;

    //Speedometer variable
    private float speedometerGM;

    public float SpeedometerGM { get { return speedometerGM; } set { speedometerGM = value; } }

    //Laps variables
    public int Laps { get { return laps; } }
    private int laps;

    public bool lapAvailable = false;

    //Score variable
    private int score;

    public int Score { get { return score; } }

    //Alive variable
    private int alive;

    public int Alive { get { return alive; } }

    //Total Alive variable
    private int totalAlive;
    public int TotalAlive { get { return totalAlive; } }

    //Nos Counter Relay variable
    private int nosCounterGM;
    public int NosCounterGM { get { return nosCounterGM; } set { nosCounterGM = value; } }

    //Camera layers
    private int cameraLayer = 1;

    //private variables
    private int scoreBonus;
    private int previousScore = 0;
    private bool gameOver = false;
    private bool paused = false;


    private void Start()
    {
        Setup();

        previousScore = (int)Time.time;

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
        if (!gameOver)
        {
            Scoreboard();
        }

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
        //Adds 1 lap to the lap counter if the debug key is pressed
        if (Input.GetKeyDown(increaseLaps))
        {
            Lap();
        }

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

    private void Scoreboard()
    {
        score = (int)Time.time + scoreBonus - previousScore;
    }

    public void Lap()
    {
        laps++;
        scoreBonus += 100;
    }

    public void Kill()
    {
        alive--;
        scoreBonus += 1000;
    }

    public void Restart()
    {
        previousScore += score;
        laps = 0;
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
        //Sets the camera LayerMask between "P1 Cam" to "P4 Cam" depending on the player ID
        obj.layer = LayerMask.NameToLayer("P" + cameraLayer + " Cam");

        //Then sets the same layer for it's children
        foreach (Transform child in obj.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("P" + cameraLayer + " Cam");
        }

        //Sets the camera CullingMask to remove the other cameras
        ChangeCameraCulling(camBrain);

        //This is just so that there are no duplicate player IDs
        cameraLayer++;
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
            if (cameraLayer != i)
            {
                cullings[index++] = LayerMask.NameToLayer("P" + i + " Cam");
            }
        }

        return cullings;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    //Screenshot Key
    [SerializeField] private KeyCode screenshotKey = KeyCode.F10;

    //Speedometer variable
    private float speedometerGM;

    public float SpeedometerGM { get { return speedometerGM; } set { speedometerGM = value; } }

    //Laps variable
    private int laps;

    public int Laps { get { return laps; } }

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

    //private variables
    private int scoreBonus;
    private int previousScore = 0;
    private bool gameOver = false;


    private void Start()
    {
        totalAlive = GameObject.FindGameObjectsWithTag("MainPlayer").Length + GameObject.FindGameObjectsWithTag("MainBot").Length;
        alive = totalAlive;

        previousScore = (int)Time.time;

        FindUI();

        //Debug
        //Time.timeScale = 0.1f;
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

    private void Lap()
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
            ui.GameOverUIRedraw();
        }
        else
        {
            FindUI();
            GameOver();
        }
    }

    private void FindUI()
    {
        ui = FindObjectOfType<UI>();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Gamemanager Instance
    private static GameManager instance;

    public static GameManager Instance { get { return instance; } }

    //Debug Keys
    [SerializeField]
    private KeyCode increaseLaps;

    [SerializeField]
    private KeyCode kill;

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

    //private variables
    private int scoreBonus;


    private void Start()
    {
        totalAlive = FindObjectsOfType<CarController>().Length;
        alive = totalAlive;
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
        Scoreboard();

        QuickDebug();
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

    private void Scoreboard()
    {
        score = (int)Time.time + scoreBonus;
        Debug.Log(scoreBonus);
    }

    private void Lap()
    {
        laps++;
        scoreBonus += 100;
    }

    private void Kill()
    {
        alive--;
        scoreBonus += 1000;
    }
}

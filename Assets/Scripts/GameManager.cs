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

    //Speedometer variable
    private float speedometerGM;

    public float SpeedometerGM { get { return speedometerGM; } set { speedometerGM = value; } }

    //Laps variable
    private int laps;

    public int Laps { get { return laps; } }


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
        Debug();
    }

    private void Debug()
    {
        //Adds 1 lap to the lap counter if the debug key is pressed
        if (Input.GetKeyDown(increaseLaps))
        {
            laps++;
        }
    }
}

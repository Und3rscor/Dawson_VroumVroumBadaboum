using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerConfigManager : MonoBehaviour
{
    //Player Setup
    private List<PlayerConfig> playerConfigs;
    public bool SpawningEnabled {  get; private set; }
    private bool spawningEnabled = true;

    //Scene Setup
    [Header("SceneSetup")]
    [SerializeField] private int gameSceneBuildIndex;

    private MainMenu mainMenu;

    public GameObject[] playerSlots;

    public PlayerConfig[] Players { get; private set; }
    private PlayerConfig[] players;

    //PCmanager Instance
    public static PlayerConfigManager Instance { get; private set; }

    //Youtube Link used to make this, PlayerConfig.cs and PlayerSetupMenuController.cs (with some tweaks of course)
    //https://www.youtube.com/watch?v=_5pOiYHJgl0&t=3s

    private void Awake()
    {
        //Instance stuff
        if (Instance != null && Instance != this)
        {
            Debug.Log("SINGLETON - homie trying to make a 2nd instance");
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
            playerConfigs = new List<PlayerConfig>();
            players = new PlayerConfig[3];
        }

    }

    public List<PlayerConfig> GetPlayerConfigs()
    {
        return playerConfigs;
    }

    public void SetPlayerColor(int index, Material mat)
    {
        playerConfigs[index].Mat = mat;
    }

    public void GrabMenu(MainMenu menu)
    {
        mainMenu = menu;
    }

    public void ReadyPlayer(int index)
    {
        playerConfigs[index].IsReady = true;
        if (playerConfigs.All(p => p.IsReady == true))
        {
            spawningEnabled = false;

            //Make the player array
            players = playerConfigs.ToArray();

            //Launches the game
            mainMenu.PlayGame();
        }
    }

    public void PlayerJoined(PlayerInput input)
    {
        if (spawningEnabled && !playerConfigs.Any(p => p.PlayerIndex == input.playerIndex))
        {
            input.transform.SetParent(transform);

            playerConfigs.Add(new PlayerConfig(input));
        }
    }

    /// <summary>
    /// Destroy all don't destroy on load objects (must be set individually)
    /// </summary>
    public void NukeAllDDOL()
    {
        Destroy(this.gameObject);
    }
}
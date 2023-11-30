using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InitializeLevel : MonoBehaviour
{
    [SerializeField] private Transform[] playerSpawns;
    [SerializeField] private GameObject playerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        PlayerConfigManager pcm = PlayerConfigManager.Instance;

        PlayerInputManager pim = pcm.GetComponent<PlayerInputManager>();

        var playerConfigs = pcm.GetPlayerConfigs().ToArray();

        pim.DisableJoining();

        InitializePlayers(playerConfigs);

        pim.splitScreen = true;
    }

    private void InitializePlayers(PlayerConfig[] playerConfigs)
    {
        for (int i = 0; i < playerConfigs.Length; i++)
        {
            var player = Instantiate(playerPrefab, playerSpawns[i].position, playerSpawns[i].rotation, gameObject.transform);

            RaceManager.Instance.PlayerSetup(player, playerConfigs[i]);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeLevel : MonoBehaviour
{
    [SerializeField] private Transform[] playerSpawns;
    [SerializeField] private GameObject playerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        var playerConfigs = PlayerConfigManager.Instance.GetPlayerConfigs().ToArray();

        for (int i = 0; i < playerConfigs.Length; i++)
        {
            var player = Instantiate(playerPrefab, playerSpawns[i].position, playerSpawns[i].rotation, gameObject.transform);

            RaceManager.Instance.PlayerSetup(player, playerConfigs[i]);
        }
    }
}

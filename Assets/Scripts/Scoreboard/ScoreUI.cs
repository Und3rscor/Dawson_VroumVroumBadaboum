using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    public RowUI rowUI;
    public ScoreManager scoreManager;

    private void Start()
    {
        var players = PlayerConfigManager.Instance.GetPlayerConfigs().ToArray();

        for (int i = 0; i < players.Count(); i++)
        {
            scoreManager.AddScore(new Score(players[i].Name, players[i].Score, players[i].Kills, players[i].FirstPlayer));
            Debug.Log(players[i].Kills);
        }

        var scores = scoreManager.GetHighScore().ToArray();
        for (int i = 0; i < scores.Length; i++)
        {
            var row = Instantiate(rowUI, transform).GetComponent<RowUI>();
            row.name.text = scores[i].name.ToString();
            row.score.text = scores[i].score.ToString();
            row.kills.text = scores[i].kills.ToString();

            if (scores[i].firstPlace)
            {
                row.name.color = Color.yellow;
            }
        }
    }
}

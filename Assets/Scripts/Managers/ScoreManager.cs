using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

[Serializable]
public class ScoreManager : MonoBehaviour
{
    public List<Score> scores;

    private void Awake()
    {
        scores = new List<Score>();
    }

    public IEnumerable<Score> GetHighScore()
    {
        return scores.OrderByDescending(x => x.score);
    }

    public void AddScore(Score score)
    {
        scores.Add(score);
    }
}

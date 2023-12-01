using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Score : MonoBehaviour
{
    public string name;
    public float score;
    public int kills;
    public bool firstPlace;

    public Score(string name, int score, int kills, bool firstPlace)
    {
        this.name = name;
        this.score = score;
        this.kills = kills;
        this.firstPlace = firstPlace;
    }
}

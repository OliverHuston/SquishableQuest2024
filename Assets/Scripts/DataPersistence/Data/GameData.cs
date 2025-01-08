using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int deathCount;


    // Default values set in constructor; the game has no data to load.
    public GameData()
    {
        this.deathCount = 0;
    }
}

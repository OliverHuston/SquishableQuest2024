using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPersistenceManager : MonoBehaviour
{
    private GameData gameData;


    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Found more than one DataPersistenceManager in the scene.");
        }
        instance = this;
    }


    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {

        if (this.gameData == null)
        {
            Debug.Log("No game data was found. Initializing data to defaults.");
            NewGame();
        }
    }

    public void SaveGame()
    {

    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}

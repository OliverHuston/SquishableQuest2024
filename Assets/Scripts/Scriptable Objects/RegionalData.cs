using System.Collections.Generic;
using UnityEngine;


public enum RegionUnlocked
{
    NOT_YET_AVAILABLE,
    COMING_SOON,
    LOCKED,
    UNLOCKED
};

[CreateAssetMenu]
public class RegionalData : ScriptableObject
{
    public string region_name;
    [TextArea] public string region_description;


    public int saveSlot;

    [Space]
    public bool available = false;
    public bool unlocked = false;
    public RegionUnlocked unlocked_status;

    [Space]
    public string[] locationNames;
    public int[] locationsProgressCodes;

    public void Save()
    {

    }

    public void Load()
    {

    }

    public void SetToDefaults()
    {

    }
}

public class LocationSaveData
{
    public string[] locationNames;
    public int[] locationsProgressCodes;
}
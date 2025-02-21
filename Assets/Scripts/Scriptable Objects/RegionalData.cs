using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RegionalData : ScriptableObject
{
    public int saveSlot;

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
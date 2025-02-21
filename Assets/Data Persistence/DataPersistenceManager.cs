using UnityEngine;

public class DataPersistenceManager : MonoBehaviour
{
    public PartyData partyData;
    public HeroStatline[] heroStatlines;
    public RegionalData[] regionalData;

    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Found more than one DataPersistenceManager in the scene.");
        }
        instance = this;
    }


    public void NewGame(int saveSlot)
    {
        AssignSaveSlot(saveSlot);
        bool dataInSaveSlot = partyData.Load();
        if (dataInSaveSlot)
        {
            Debug.Log("Cannot create New Game. There is already data in Save Slot " + saveSlot + ".");
        }
        else
        {
            SetAllToDefaults();
        }
    }

    public void LoadGame(int saveSlot)
    {
        AssignSaveSlot(saveSlot);
        bool dataInSaveSlot = partyData.Load();

        if (dataInSaveSlot)
        {
            foreach (HeroStatline hs in heroStatlines) { hs.Load(); }
            foreach (RegionalData rd in regionalData) { rd.Load(); }
        }
        else
        {
            Debug.Log("Cannot load. No save data found in Save Slot " + saveSlot + ".");
        }
    }

    public void LoadGame()
    {
        LoadGame(partyData.saveSlot);
    }

    public void SaveGame()
    {
        partyData.Save();
        foreach (HeroStatline hs in heroStatlines) { hs.Save(); }
        foreach (RegionalData rd in regionalData) { rd.Save(); }
    }

    public bool hasData(int saveSlot)
    {
        partyData.saveSlot = saveSlot;
        return partyData.Load();
    }

    public string getPartyName(int saveSlot)
    {
        partyData.saveSlot = saveSlot;
        partyData.Load();
        return partyData.partyName;
    }

    private void AssignSaveSlot(int saveSlot)
    {
        partyData.saveSlot = saveSlot;
        foreach(HeroStatline hs in heroStatlines) { hs.saveSlot = saveSlot; }
        foreach(RegionalData rd in regionalData) { rd.saveSlot = saveSlot; }
    }

    private void SetAllToDefaults()
    {
        partyData.SetToDefaults();
        foreach (HeroStatline hs in heroStatlines) { hs.SetToDefaults(); }
        foreach (RegionalData rd in regionalData) { rd.SetToDefaults(); }
    }


    private void OnApplicationQuit()
    {
        // Save game on exit, assuming the party has been fully created.
        if(partyData.partyName != null) { SaveGame(); }
    }
}

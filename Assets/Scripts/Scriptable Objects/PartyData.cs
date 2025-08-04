using UnityEngine;

[CreateAssetMenu]
public class PartyData : ScriptableObject
{
    public int saveSlot;

    [Space]
    public string partyName;
    public int gold;
    public Item[] inventory;


    public void Save()
    {
        PartySaveData partySaveData = new PartySaveData();

        // Copy data to PartySaveData object.
        partySaveData.partyName = this.partyName;
        partySaveData.gold = this.gold;
        partySaveData.inventory = new Item[this.inventory.Length];
        this.inventory.CopyTo(partySaveData.inventory, 0);

        // Save to Json.
        string saveData = JsonUtility.ToJson(partySaveData);
        string filepath = Application.persistentDataPath + "/" + this.saveSlot + "_party.json";
        System.IO.File.WriteAllText(filepath, saveData);
        Debug.Log("Saved party data in Save Slot " + this.saveSlot + ".");
    }

    public bool Load()
    {
        try
        {
            // Load from Json.
            string filepath = Application.persistentDataPath + "/" + saveSlot + "_party.json";

            string data = System.IO.File.ReadAllText(filepath);
            PartySaveData partySaveData = JsonUtility.FromJson<PartySaveData>(data);

            // Copy data from PartySaveData object.
            this.partyName = partySaveData.partyName;
            this.gold = partySaveData.gold;
            this.inventory = new Item[partySaveData.inventory.Length];
            partySaveData.inventory.CopyTo(this.inventory, 0);
            Debug.Log("Found and loaded party data from Save Slot " + this.saveSlot + ".");
            return true;
        }
        catch
        {
            Debug.Log("No party data found in Save Slot " + this.saveSlot + ".");
            return false;
        }
    }


    public void SetToDefaults()
    {
        partyName = null;
        gold = 0;
        inventory = new Item[48];
    }
}

public class PartySaveData
{
    public string partyName;
    public int gold;
    public Item[] inventory;
}
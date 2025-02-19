using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[CreateAssetMenu]
public class HeroStatline : CharacterStatline
{
    // Loading Sources
    private string levelMatrixFolder = "Assets/Hero Level Matrices/";

    [Space]
    public int saveSlot = 0;

    [Space]
    public int level = 0;
    public int xp = 0;
    public Item[] commonInv = new Item[4];
    public Item[] uncommonInv = new Item[4];
    public Item[] rareInv = new Item[4];


    public void LoadStatline()
    {
        Debug.Log("Loading...");
        LoadStatsFromJson();
        LoadStatsFromLevelMatrix(this.level);
    }

    public void SaveStatline()
    {
        HeroSaveData heroSaveData = new HeroSaveData();

        // Copy statline data to save object.
        heroSaveData.level = this.level;
        heroSaveData.maxHealth = this.health;
        heroSaveData.xp = this.xp;
        heroSaveData.skills = new List<Skill>(this.skills);
        heroSaveData.commonInv = new Item[4];
        this.commonInv.CopyTo(heroSaveData.commonInv, 0);
        heroSaveData.uncommonInv = new Item[4];
        this.uncommonInv.CopyTo(heroSaveData.uncommonInv, 0);
        heroSaveData.rareInv = new Item[4];

        this.rareInv.CopyTo(heroSaveData.rareInv, 0);

        // Save to Json
        string inventoryData = JsonUtility.ToJson(heroSaveData);
        string filepath = Application.persistentDataPath + "/"+saveSlot+"_"+this.displayName+".json";
        System.IO.File.WriteAllText(filepath, inventoryData);
        Debug.Log("Saved.");
    }
    private void LoadStatsFromJson()
    {
        string filepath = Application.persistentDataPath + "/" + saveSlot + "_" + this.displayName + ".json";
        string inventoryData = System.IO.File.ReadAllText(filepath);
        HeroSaveData heroSaveData = JsonUtility.FromJson<HeroSaveData>(inventoryData);

        // Copy from save object to statline.
        this.level = heroSaveData.level;
        this.health = heroSaveData.maxHealth;
        this.xp = heroSaveData.xp;
        this.skills = new List<Skill>(heroSaveData.skills);
        heroSaveData.commonInv.CopyTo(this.commonInv, 0);
        heroSaveData.uncommonInv.CopyTo(this.uncommonInv, 0);
        heroSaveData.rareInv.CopyTo(this.rareInv, 0);
    }
    private void LoadStatsFromLevelMatrix(int level)
    {
        StreamReader strReader = new StreamReader(levelMatrixFolder+this.displayName+ "_LevelMatrix.csv");
        bool reading = true;
        while (reading)
        {
            string data_string = strReader.ReadLine();
            if (data_string == null)
            {
                reading = false;
                Debug.Log("Level not found in matrix. No stats assigned.");
                break;
            }

            var data_values = data_string.Split(',');

            //  Reading data in from loaded level.
            if (data_values[0].ToString().Equals(level.ToString()))
            {
                this.moves = int.Parse(data_values[2]);
                this.weaponskill = int.Parse(data_values[3]);
                this.ballisticskill = int.Parse(data_values[4]);
                this.strength = int.Parse(data_values[5]);
                this.toughness = int.Parse(data_values[6]);
                this.initiative = int.Parse(data_values[7]);
                this.meleeAttacks = int.Parse(data_values[8]);
                this.rangedAttacks = int.Parse(data_values[9]);


                reading = false;
                break;
            }
        }
    }
 
}

[System.Serializable]
public class HeroSaveData
{
    public int level;
    public int maxHealth;
    public int xp;

    public List<Skill> skills;

    public Item[] commonInv;
    public Item[] uncommonInv;
    public Item[] rareInv;
}
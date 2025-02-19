using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[CreateAssetMenu]
public class HeroStatline : CharacterStatline
{
    public string levelMatrixAddress = "";

    public int xp = 0;

    public Item[] commonInv = new Item[4];
    public Item[] uncommonInv = new Item[4];
    public Item[] rareInv = new Item[4];



    public void LoadStatline()
    {
        //temp 
        int loaded_lvl = 10;
        int loaded_maxHealth = 12;
        int loaded_xp = 0;

        string[] loaded_Skills;
        string[] loaded_commonInv;
        string[] loaded_uncommonInv;
        string[] loaded_rareInv;



        LoadStatsFromLevelMatrix(loaded_lvl);
    }

    public void SaveStatline()
    {

    }

    private void LoadStatsFromLevelMatrix(int level)
    {
        StreamReader strReader = new StreamReader(levelMatrixAddress);
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
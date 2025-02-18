using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class HeroSaveAsset : ScriptableObject
{
    [Tooltip("Levels go from 1-10. Lvl. 0 signifies a character not yet created.")]
    public int level = 0;
    public int xp = 0;
    public int maxHealth = 0;

    [Space]
    public List<string> skills;

    [Space]
    public string[] commonInv = new string[4];
    public string[] uncommonInv = new string[4];
    public string[] rareInv = new string[4];
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Rarity
{
    COMMON = 0,
    UNCOMMON = 1,
    RARE = 2
}

public enum ItemSlot
{
    NONE = 0,
    ONEHANDED_WEAPON = 1,
    TWOHANDED_WEAPON = 2,
    RANGED_WEAPON = 3,
    SINGLE_USE = 4,
    ONCE_PER_DUNGEON = 5,
    ARMOR = 6,
    HELMET = 7,
    SHIELD = 8
}

public class Item : ScriptableObject
{
    public string itemName;

    [Space]
    public Rarity rarity;
    [TextArea] public string displayDescription;

    public string effectCodes;
}

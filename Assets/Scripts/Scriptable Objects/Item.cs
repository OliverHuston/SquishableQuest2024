using UnityEngine;
using UnityEngine.UI;

public enum ItemRarity
{
    COMMON = 0,
    UNCOMMON = 1,
    RARE = 2
}

public enum ItemType
{
    NONE = 0,
    ONEHANDED_WEAPON = 1,
    TWOHANDED_WEAPON = 2,
    RANGED_WEAPON = 3,
    SINGLE_USE = 4,
    ONCE_PER_TURN = 5,
    ONCE_PER_DUNGEON = 6,
    ARMOR = 7,
    HELMET = 8,
    SHIELD = 9
}

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public string itemName;

    public Sprite displayPNG;

    [Space]
    public ItemRarity rarity;
    public ItemType type;
    [TextArea] public string displayDescription;

    [Space]
    public string effectCode;
}
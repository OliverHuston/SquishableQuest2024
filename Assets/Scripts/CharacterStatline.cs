using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharacterStatline : ScriptableObject
{
    // Turn action allowances:
    public int moves = 4;
    public int meleeAttacks = 1;
    public int rangedAttacks = 0;

    // Health
    public int health = 10;

    // Fixed stats
    [Tooltip("Compared with enemy WS to determine melee hit chance")]
    public int weaponskill = 3;
    [Tooltip("Determines ranged hit chance, subject to modifiers for range, moving, etc.")]
    public int ballisticskill = 3;
    [Tooltip("Determines base wound chance for melee")]
    public int strength = 3;
    [Tooltip("Determines base wound resistance against attacks")]
    public int toughness = 3;
    [Tooltip("Determines pinning and dodge chances")]
    public int initiative = 3;
}

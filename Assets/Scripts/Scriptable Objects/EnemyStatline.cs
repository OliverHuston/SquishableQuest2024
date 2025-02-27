using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum EnemyBehavior
{
    DEFAULT_MELEE = 0,  // move toward chosen target, attack that target
    DEFAULT_RANGED = 1
}

public enum EnemySpawn
{
    ADJACENT = 0,
    AT_DISTANCE = 1
}


[CreateAssetMenu]
public class EnemyStatline : CharacterStatline
{
    public int gold = 10;
    public int armor = 0;
    public string damage = "d6";

    public EnemyBehavior enemyBehavior;
    public EnemySpawn enemySpawn;
}

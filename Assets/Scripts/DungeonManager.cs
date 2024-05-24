using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using UnityEngine;

public enum Turn
{
    PLAYER = 0,
    ENEMIES = 1,
    POPUP = 2
}

public enum AttackType
{
    MELEE = 0,
    RANGED = 1
}


public class DungeonManager : MonoBehaviour
{
    public Turn turn;
    public GameObject endTurnButton;
    public Character[] characters;

    // Start is called before the first frame update
    void Awake()
    {
        characters = FindObjectsOfType<Character>();  //needs fixing to exclude enemies
        endTurnButton.SetActive(false);
    }

    //-----------------------------------------------------------------------------------------------------------------//
    //***TURN MANAGEMENT***
    public void EndTurn()
    {
        endTurnButton.SetActive(false);
        turn = Turn.ENEMIES;
        EnemyTurn();
    }
    private void EnemyTurn()
    {
        //work here

        turn = Turn.PLAYER;
        StartPlayerTurn();
        return;
    }
    private void StartPlayerTurn()
    {
        foreach (Character c in characters)
        {
            if (c.characterType == CharacterType.HERO) c.ResetActionAllowances();
        }
    }

    //-----------------------------------------------------------------------------------------------------------------//
    public void CellManagerStatusUpdate(SelectionPhase selectionPhase)
    {
        if (selectionPhase == SelectionPhase.CHOOSE_CHARACTER) { endTurnButton.SetActive(true); }
        else { endTurnButton.SetActive(false); }
    }

    //-----------------------------------------------------------------------------------------------------------------//
    //  (called from CellManager when processing click)
    public void ProcessAttack(Character attacker, Character defender, AttackType attackType)
    {
        // Rotate to target
        StartCoroutine(attacker.RotateToFace(defender.cell));

        // To hit
        if (attackType == AttackType.MELEE)
        {
            if (!Roll(7 - attacker.statline.weaponskill)) {
                Debug.Log("MISSED MELEE!");
                return;
            }
        }
        else if (attackType == AttackType.RANGED)
        {
            if (!Roll(7 - attacker.statline.ballisticskill)) {
                Debug.Log("MISSED SHOOTING!");
                return; 
            }
        }

        // To wound


        // Inflict damage
        //(temp)
        defender.currentHealth -= 2;


        // Check for deathblow if melee

    }

    //-----------------------------------------------------------------------------------------------------------------//
    //***DICE ROLLING FUNCTIONS***

    // Roll an X+. 1 always fails, 6 always succeeds.
    public bool Roll(int target)
    {
        if (target < 2) target = 2;
        else if (target > 6) target = 6;

        if (D6() >= target) return true;
        return false;
    }

    public int D6()
    {
        int roll = (int)Random.Range(0, 5) + 1;
        return roll;
    }

}

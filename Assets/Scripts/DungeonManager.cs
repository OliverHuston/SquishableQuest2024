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
    // UI elements
    public GameObject endTurnButton;
    public AnimatedText attackDisplayText;
    public StatsDisplayPanel heroDisplayPanel;
    public StatsDisplayPanel enemyDisplayPanel;

    // Status vars
    [HideInInspector] public Turn turn;
    
    // Convenience reference
    [HideInInspector] public Character[] characters;
    [HideInInspector] public Camera mainCamera;

    // Charts
    private int[,] ToHitChart =                 //attacker, defender
        {{4, 4, 5, 6, 6, 6, 6, 6, 6, 6, 6},
         {3, 4, 4, 4, 5, 5, 6, 6, 6, 6, 6},
         {2, 3, 4, 4, 4, 4, 4, 5, 5, 5, 6},
         {2, 3, 3, 4, 4, 4, 4, 4, 4, 5, 5},
         {2, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4},
         {2, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4},
         {2, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4},
         {2, 2, 3, 3, 3, 3, 3, 3, 4, 4, 4},
         {2, 2, 2, 3, 3, 3, 3, 3, 3, 4, 4},
         {2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 4}};


    void Awake()
    {
        characters = FindObjectsOfType<Character>();  //needs fixing to exclude enemies
        mainCamera = Camera.main;
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
        foreach (Character c in characters)
        {
            if (c.characterType == CharacterType.ENEMY) c.ResetActionAllowances();
        }
        foreach (Character c in characters)
        {
            if (c.characterType == CharacterType.ENEMY) c.TakeTurn();
        }

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
    //***UI MANAGEMENT***
    public void CellManagerStatusUpdate(SelectionPhase selectionPhase)
    {
        if (selectionPhase == SelectionPhase.CHOOSE_CHARACTER) { 
            endTurnButton.SetActive(true);
/*            heroDisplayPanel.gameObject.SetActive(false);
            enemyDisplayPanel.gameObject.SetActive(false);*/
        }
        else { endTurnButton.SetActive(false); }
    }
    public void DisplayCharacterStats(Character character)
    {
        if (character == null)
        {
            heroDisplayPanel.gameObject.SetActive(false);
            enemyDisplayPanel.gameObject.SetActive(false);

        }
        else if(character.characterType == CharacterType.HERO)
        {
            heroDisplayPanel.gameObject.SetActive(true);
            heroDisplayPanel.UpdateStats(character);
        }
        else if(character.characterType == CharacterType.ENEMY)
        {
            enemyDisplayPanel.gameObject.SetActive(true);
            enemyDisplayPanel.UpdateStats(character);
        }
    }
    

    //-----------------------------------------------------------------------------------------------------------------//
    //***ATTACK MANAGEMENT***
    public void ProcessAttack(Character attacker, Character defender, AttackType attackType)
    {
        // Rotate to target
        StartCoroutine(attacker.RotateToFace(defender.cell));

        // Roll to hit
        if (attackType == AttackType.MELEE && !Roll(ToHitChart[attacker.statline.weaponskill, defender.statline.weaponskill]))
        {
            MissedText(defender);
            return;
        }
        else if (attackType == AttackType.RANGED && !Roll(7 - attacker.statline.ballisticskill))
        {
            MissedText(defender);
            return;
        }

        // Inflict damage
        int weapon_damage = ParseDiceString(attacker.GetDamage());
        int damage = weapon_damage + attacker.statline.strength - defender.statline.toughness;
        if (true) damage -= defender.GetArmor(); //temp: some weapons/abilities ignore armor
        damage = Mathf.Max(1, damage); // every hit should do at least one damage, regardless of any modifiers

        DamageText(defender, damage);
        defender.currentHealth -= damage;

        // Check for deathblow if melee [NEEDS work]
        //....
    }
    // Attack UI display functions
    private void DamageText(Character damagedCharacter, int damage)
    {
        attackDisplayText.gameObject.transform.position = mainCamera.WorldToScreenPoint(damagedCharacter.gameObject.transform.position);
        attackDisplayText.gameObject.SetActive(true);
        attackDisplayText.SetMessage("-" + damage);
        attackDisplayText.SetColor(Color.white);        
        StartCoroutine(attackDisplayText.FadeInAndOut(.6f, 1f));
        attackDisplayText.gameObject.SetActive(true);
    }
    private void MissedText(Character missedCharacter)
    {
        attackDisplayText.gameObject.transform.position = mainCamera.WorldToScreenPoint(missedCharacter.gameObject.transform.position);
        attackDisplayText.gameObject.SetActive(true);
        attackDisplayText.SetMessage("MISSED!");
        attackDisplayText.SetColor(Color.white);
        StartCoroutine(attackDisplayText.FadeInAndOut(.6f, 1f));
        attackDisplayText.gameObject.SetActive(true);
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

    // NEEDS work for parsing 2d6, 3d6, d6 + 1 etc.
    public int ParseDiceString (string dice)
    {
        int result = 0;
        if (dice == null) return 0;

        if(dice.Equals("d6")) { result = D6(); }
        else if(dice.Equals("d3")) { result = D3(); }
        else if (dice.Equals("d66")) { result = D66(); }
        return result;
    }

    // Returns a d6 value.
    public int D6()
    {
        int roll = (int)Random.Range(0, 5) + 1;
        return roll;
    }
    public int D3()
    {
        int roll = (int)Random.Range(0, 2) + 1;
        return roll;
    }
    public int D66()
    {
        return (D6() * 10) + D6();
    }
}

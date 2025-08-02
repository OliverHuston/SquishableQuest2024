using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using UnityEngine;

public enum Turn
{
    START = 0,
    HERO = 1,
    MONSTER = 2,
    END = 4
}

public enum AttackType
{
    MELEE = 0,
    RANGED = 1
}
 

public class DungeonManager : MonoBehaviour
{
    // UI instanced elements
    public Transform dungeonUI_transform;
    public GameObject endTurnButton;
    public StatsDisplayPanel heroDisplayPanel;
    public StatsDisplayPanel enemyDisplayPanel;

    // UI Prefabs
    public AnimatedText actionAnimatedText;

    // Status variables
    [HideInInspector] public Turn turn;
    
    // Reference variables
    [HideInInspector] public Character[] characters;

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

    //-----------------------------------------------------------------------------------------------------------------//
    //***SETUP***
    public static DungeonManager instance { get; private set; }
    void Awake()
    {
        if (instance != null) Debug.LogError("Found more than one DungeonManager in the scene.");
        instance = this;

        characters = FindObjectsOfType<Character>();  //needs fixing to exclude enemies
        endTurnButton.SetActive(false);

        StartCoroutine(StartOfTurnPhase());
    }

    //-----------------------------------------------------------------------------------------------------------------//
    //***TURN MANAGEMENT***
    private IEnumerator StartOfTurnPhase()
    {
        turn = Turn.START;

        //StartOfTurnCodeHere
        //NB: when first turn starts, ambush special event shouldn't be allowed (not allowed until at least on room explored)

        StartHeroPhase();
        yield return null;
    }

    private void StartHeroPhase()
    {
        turn = Turn.HERO;
        foreach (Character c in characters)
        {
            if (c.characterType == CharacterType.HERO) c.ResetActionAllowances();
        }
    }

    // EndHeroPhase called by the NextTurnButton
    public void EndHeroPhase()
    {
        endTurnButton.SetActive(false);
        StartCoroutine(MonsterPhase());
    }

    private IEnumerator MonsterPhase()
    {
        turn = Turn.MONSTER;

        foreach (Character c in characters)
        {
            if (c.characterType == CharacterType.ENEMY) c.ResetActionAllowances();
        }
        foreach (Character c in characters)
        {
            if (c.characterType == CharacterType.ENEMY) yield return c.TakeTurn();
        }

        StartCoroutine(EndOfTurnPhase());
        yield return null;
    }

    private IEnumerator EndOfTurnPhase()
    {
        turn = Turn.END;

        //EndOfTurn code here

        StartCoroutine(StartOfTurnPhase());
        yield return null;
    }



    //-----------------------------------------------------------------------------------------------------------------//
    //***UI MANAGEMENT***
    public void CellManagerStatusUpdate(SelectionPhase selectionPhase)
    {
        if (selectionPhase == SelectionPhase.CHOOSE_CHARACTER) { 
            endTurnButton.SetActive(true);
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

    // Used to display missed or damage amount for attacks against a target a characterLocation.
    private void ActionText(Character characterLocation, string message)
    {
        AnimatedText animatedText = Instantiate(actionAnimatedText,
            Camera.main.WorldToScreenPoint(characterLocation.gameObject.transform.position),
            Quaternion.identity, dungeonUI_transform);
        StartCoroutine(animatedText.PlayMessageAndDestroy(message, Color.white, .6f, 1.0f));
    }


    //-----------------------------------------------------------------------------------------------------------------//
    //***ATTACK MANAGEMENT***
    public void ProcessAttack(Character attacker, Character defender, AttackType attackType)
    {
        attacker.remainingMoves = 0; //no moves after attacking
        if(attackType == AttackType.MELEE) { attacker.remainingMeleeAttacks--; }
        else if (attackType == AttackType.RANGED) { attacker.remainingRangedAttacks--; }


        // Rotate to target
        StartCoroutine(attacker.RotateToFace(defender.cell));

        // Roll to hit
        if (attackType == AttackType.MELEE && !Roll(ToHitChart[attacker.statline.weaponskill, defender.statline.weaponskill]))
        {
            ActionText(defender, "MISSED!");
            return;
        }
        else if (attackType == AttackType.RANGED && !Roll(7 - attacker.statline.ballisticskill))
        {
            ActionText(defender, "MISSED!");
            return;
        }

        Debug.Log(attacker.name + " hit " + defender.name);

        // Inflict damage
        int attacker_damage = ParseDiceString(attacker.GetDamage(attackType));
        int damage = attacker_damage - defender.toughness;
        if (true) damage -= defender.GetArmor(); //temp: some weapons/abilities ignore armor
        damage = Mathf.Max(1, damage); // every hit should do at least one damage, regardless of any modifiers

        Debug.Log("for " + damage);

        ActionText(defender, "-" + damage);
        defender.currentHealth -= damage;

        // Check for deathblow if melee [NEEDS work]
        //....
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
        if (dice == null) return 0;

        while (dice.Contains('d'))
        {
            dice = dice.Replace("d66", D66() + "");
            dice = dice.Replace("d6", D6() + "");
            dice = dice.Replace("d3", D3() + "");
        }

        return StringFormulaToInt(dice); // function needs work
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

    // NEEDS WORK for multi digit nums and mult/div
    public int StringFormulaToInt(string s)
    {
        int result = 0;
        int sign = 1;
        s.Replace(" ", "");
        for(int i = 0; i < s.Length; i++)
        {
            if (s[i] == '+') { sign = 1; }
            else if (s[i] == '-') { sign = -1; }
            else if (char.IsNumber(s[i])) {
                result += sign * int.Parse(s[i]+"");
            }
        }
        return result;
    }
}

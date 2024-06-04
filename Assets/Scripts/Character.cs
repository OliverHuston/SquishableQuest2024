using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;


public class Character : MonoBehaviour
{
    //***VARIABLES***
    #region
    // Statline assignment
    //public HeroStatline heroStatline;
    public EnemyStatline enemyStatline;
    public CharacterStatline statline;

    // Stats and display settings (from this.statline)
    [HideInInspector] public string characterDisplayName;
    [HideInInspector] public Sprite characterDisplayPortrait;
    [HideInInspector] public CharacterType characterType;
    [HideInInspector] public int weaponskill;
    [HideInInspector] public int ballisticskill;
    [HideInInspector] public int strength;
    [HideInInspector] public int toughness;
    [HideInInspector] public int initiative;
    [HideInInspector] public int maxHealth;

    // Current stat values
    [HideInInspector] public int currentHealth;
    [HideInInspector] public int remainingMoves;
    [HideInInspector] public int remainingMeleeAttacks;
    [HideInInspector] public int remainingRangedAttacks;

    // Position and status variables
    [HideInInspector] public int xPos;
    [HideInInspector] public int yPos;
    [HideInInspector] public Cell cell;
    [HideInInspector] public bool available = true;

    // Move anim parameters (should be the same across all characters)
    public float moveAnimSpeed = 2f;
    public float rotateAnimSpeed = 4f;
    public float moveDestinationTolerance = .1f;
    public float rotateAngleTolerance = .1f;

    // Reference variables
    private MapManager mapManager;
    private CellManager cellManager;
    private DungeonManager dungeonManager;
    #endregion

    //***SETUP***
    #region
    void Awake()
    {
        // 1. Assign reference variables.
        mapManager = FindObjectOfType<MapManager>();
        cellManager = FindObjectOfType<CellManager>();
        dungeonManager = FindObjectOfType<DungeonManager>();

        // 2. Assign statline based on provided EnemyStatline or HeroStatline.
        if(enemyStatline != null ) {
            statline = enemyStatline;
            statline.characterType = CharacterType.ENEMY;
        }

        // 3. Assign stats based on statline.
        characterDisplayName = statline.displayName;
        characterDisplayPortrait = statline.portrait;
        characterType = statline.characterType;
        weaponskill = statline.weaponskill;
        ballisticskill = statline.ballisticskill;
        strength = statline.strength;
        toughness = statline.toughness;
        initiative = statline.initiative;
        maxHealth = statline.health;

        // 4. Set current stat values to full and set starting position.
        currentHealth = statline.health;
        ResetActionAllowances();
        OccupyCell();
        xPos = (int)transform.position.x;
        yPos = (int)transform.position.z; 
    }
    #endregion

    void FixedUpdate()
    {
        // Update occupied cell connection
        OccupyCell();
    }


    //-----------------------------------------------------------------------------------------------------------------//
    // Set this.cell and cell.occupant as appropriate after changing cells.
    private void OccupyCell()
    {
        // Return if cell remains the same
        if (cell != null)
        {
            if (cell.x == xPos && cell.y == yPos) return;
        }
        
        // Character changed cells. Clear old cell's occupant. Set new cell occupant and set new cell to cell.
        Cell new_cell = cellManager.FindCell(xPos, yPos);
        if (new_cell == null) { cell = null; return; }

        if (cell != null) { cell.occupant = null; }
        new_cell.occupant = this.GetComponent<Character>();
        cell = new_cell;
        return;
    }
    // Reset moves, melee attack, and ranged attacks (called at start of a new turn).
    public void ResetActionAllowances()
    {
        remainingMoves = statline.moves;
        remainingMeleeAttacks = statline.meleeAttacks;
        remainingRangedAttacks = statline.rangedAttacks;
    }

    //-----------------------------------------------------------------------------------------------------------------//
    //***STAT REPORTING*** (method varies depending on whether character is using an EnemyStatline or a HeroStatline

    public string GetDamage(AttackType attackType)
    {
        if (characterType == CharacterType.ENEMY) {
            if(attackType == AttackType.MELEE)
            {
                return enemyStatline.damage + "+" + this.strength;
            }
            else if(attackType == AttackType.RANGED)
            {
                return FindRuleValue("Bow");
            }
        }

        // Default d6 damage + strength.
        return "d6" + "+" + this.strength;
    }
    public int GetArmor()
    {
        if (characterType == CharacterType.ENEMY) return enemyStatline.armor;
        return 0;
    }

    //-----------------------------------------------------------------------------------------------------------------//
    //***ENEMY BEHAVIOR***
    // Enemy character takes turn (called in EnemyTurn() in dungeonManager).
    public IEnumerator TakeTurn()
    {
        if (characterType != CharacterType.ENEMY) yield return null;
        yield return new WaitForSeconds(1f);
        yield break; // temp

        // Choosing target
        Character target = enemyStatline.target;
        if (target == null)
        {
            target = GameObject.Find("Character").GetComponent<Character>();

            if (enemyStatline.enemyBehavior == EnemyBehavior.DEFAULT_MELEE)
            {
                
            }
        }

        // Move behavior
        if (enemyStatline.enemyBehavior == EnemyBehavior.DEFAULT_MELEE)
        {
            yield return MoveTowardTarget(target);
        }
        else if (enemyStatline.enemyBehavior == EnemyBehavior.DEFAULT_RANGED)
        { }

        // Attack behavior
        MakeAllAttacks(target);
    }
    private IEnumerator MoveTowardTarget(Character target)
    {
        cellManager.MoveCharacterToCell(this, target.cell, true);
        //while (this.available == false) {}
        yield return null;
    }
    private void MakeAllAttacks(Character target)
    {
        while(Attack(target)) { }
        //NEEDS WORK: allow temp target switching to use up all melee and ranged attacks
    }



    //-----------------------------------------------------------------------------------------------------------------//
    private string FindRuleValue(string ruleName)
    {
        if (this.characterType != CharacterType.ENEMY) return "";

        foreach(string rule in enemyStatline.specialRules)
        {
            if (rule.Contains(ruleName))
            {
                string value = rule.Substring(rule.IndexOf('(')+1, rule.Length-(rule.IndexOf('(') + 1) - 1);
                return value;
            }
        }
        return "";
    }

    //-----------------------------------------------------------------------------------------------------------------//
    //***ACTIONS***
    public bool Attack(Character target)
    {
        AttackType attackType = AttackType.RANGED;
        if (cellManager.IsAdjacent(this.cell, target.cell)) { attackType = AttackType.MELEE; }
        if((attackType == AttackType.MELEE && remainingMeleeAttacks < 1) 
            || (attackType == AttackType.RANGED && (remainingRangedAttacks < 1 || cellManager.LineOfSight(this.cell, target.cell)))) 
            { return false; }

        dungeonManager.ProcessAttack(this, target, attackType);
        return true;
    }

    //-----------------------------------------------------------------------------------------------------------------//
    //***MOVE ANIMATIONS***
    #region
    // Move and rotate along provided cell path.
    public IEnumerator MoveAlongPath(Cell[] path)
    {
        this.available = false;
        Cell destination = path[path.Length-1];
        int iterations = path.Length;
        if (destination.occupant != null) { 
            destination = path[path.Length-2];
            iterations--;
        }
        xPos = destination.x; 
        yPos = destination.y;
        OccupyCell();

        for(int i = 1; i < iterations; i++)
        {
            yield return RotateAnimation(path[i].x, path[i].y);
            yield return MoveAnimation(path[i].x, path[i].y);
        }

        // Upon arriving in cell
        if (this.cell.cell_code == 'o') { mapManager.ExploreRoom(this.cell.room, this.cell.exitCode); }
        this.available = true;
    }
    // Rotate to face a provided cell.
    public IEnumerator RotateToFace(Cell cell)
    {
        yield return RotateAnimation(cell.x, cell.y);
    }
    // Rotate towards point (x, y).
    private IEnumerator RotateAnimation (int x, int y)
    {
        Vector3 targetPosition = new Vector3(x, transform.position.y, y);
        Vector3 targetDirection = (targetPosition - transform.position).normalized;

        float angleDistance = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(targetDirection));
        while (angleDistance > rotateAngleTolerance)
        {
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotateAnimSpeed * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);

            angleDistance = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(targetDirection));
            yield return new WaitForFixedUpdate();
        }

        transform.rotation = Quaternion.LookRotation(targetDirection);
        yield break;
    }
    // Move towards point (x, y). Character must already be rotated to face the point.
    private IEnumerator MoveAnimation(int x, int y)
    {
        Vector3 targetPosition = new Vector3(x, transform.position.y, y);

        float distance = Vector3.Distance(transform.position, targetPosition);
        while (distance > moveDestinationTolerance)
        {
            transform.Translate(Vector3.forward * moveAnimSpeed * Time.deltaTime);
            
            distance = Vector3.Distance(transform.position, targetPosition);
            yield return new WaitForFixedUpdate();
        }

        transform.position = targetPosition;
        yield break;
    }
    #endregion
}

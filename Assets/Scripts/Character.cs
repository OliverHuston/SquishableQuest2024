using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using static Unity.VisualScripting.Member;



public class Character : MonoBehaviour
{
    // Settings variables
    //public HeroStatline heroStatline;
    public EnemyStatline enemyStatline;

    public CharacterStatline statline;

    // Display Variables
    [HideInInspector] public string characterDisplayName;
    [HideInInspector] public Texture characterDisplayPortrait;

    // Current variable statuses
    [HideInInspector] public int currentHealth;
    [HideInInspector] public int remainingMoves;
    [HideInInspector] public int remainingMeleeAttacks;
    [HideInInspector] public int remainingRangedAttacks;

    // Stats from this.statline
    [HideInInspector] public CharacterType characterType;
    [HideInInspector] public int weaponskill;
    [HideInInspector] public int ballisticskill;
    [HideInInspector] public int strength;
    [HideInInspector] public int toughness;
    [HideInInspector] public int initiative;
    [HideInInspector] public int maxHealth;

    // Move anim parameters (hardcoded)
    public float moveAnimSpeed = 2f;
    public float rotateAnimSpeed = 4f;
    public float moveDestinationTolerance = .1f;
    public float rotateAngleTolerance = .1f;

    // Status variables
    [HideInInspector] public int xPos;
    [HideInInspector] public int yPos;
    [HideInInspector] public Cell cell;


    // Convenience
    private CellManager cellManager;
    private DungeonManager dungeonManager;


    void Awake()
    {
        cellManager = FindObjectOfType<CellManager>();
        dungeonManager = FindObjectOfType<DungeonManager>();

        if(enemyStatline != null ) { statline = enemyStatline; }

        characterDisplayName = statline.displayName;
        characterDisplayPortrait = statline.portrait;

        currentHealth = statline.health;
        ResetActionAllowances();
        OccupyCell();
        xPos = (int)transform.position.x;
        yPos = (int)transform.position.z;

        characterType = statline.characterType;
        weaponskill = statline.weaponskill;
        ballisticskill = statline.ballisticskill;
        strength = statline.strength;
        toughness = statline.toughness;
        initiative = statline.initiative;
        maxHealth = statline.health;
    }

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

    public string GetDamage()
    {
        if (characterType == CharacterType.ENEMY) return enemyStatline.damage;

        // Default d6 damage.
        return "d6";
    }
    public int GetArmor()
    {
        if (characterType == CharacterType.ENEMY) return enemyStatline.armor;
        return 0;
    }

    //-----------------------------------------------------------------------------------------------------------------//
    //***ENEMY BEHAVIOR***
    public void TakeTurn()
    {
        if (characterType != CharacterType.ENEMY) return;

        // Choosing target
        Character target = enemyStatline.target;
        if (target == null)
        {
            if(enemyStatline.enemyBehavior == EnemyBehavior.DEFAULT_MELEE)
            {
                target = FindFirstObjectByType<Character>(); //temp
                Debug.Log(target.name); //temp
            }
        }

        // Move behavior
        if (enemyStatline.enemyBehavior == EnemyBehavior.DEFAULT_MELEE)
        {
            MoveTowardTarget(target);
        }
        else if (enemyStatline.enemyBehavior == EnemyBehavior.DEFAULT_RANGED)
        {

        }

        // Attack behavior
        MakeAllAttacks(target);
    }
    private void MoveTowardTarget(Character target)
    {
        cellManager.MoveCharacterToCell(this, target.cell, true);

    }
    private void MakeAllAttacks(Character target)
    {
        while(Attack(target)) { }
        //NEEDS WORK: allow temp target switching to use up all melee and ranged attacks
    }


    //-----------------------------------------------------------------------------------------------------------------//
    //***ACTIONS***
    public bool Attack(Character target)
    {
        AttackType attackType = AttackType.RANGED;
        if (cellManager.IsAdjacent(this.cell, target.cell)) { attackType = AttackType.MELEE; }
        if((attackType == AttackType.MELEE && remainingMeleeAttacks < 1) 
            || (attackType == AttackType.RANGED && remainingRangedAttacks < 1)) { return false; }

        dungeonManager.ProcessAttack(this, target, attackType);
        return true;
    }

    //-----------------------------------------------------------------------------------------------------------------//
    //***MOVE ANIMATIONS***
    // Move and rotate along provided cell path.
    public IEnumerator MoveAlongPath(Cell[] path)
    {
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
}

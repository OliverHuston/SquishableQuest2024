using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum CharacterType
{
    HERO = 0,
    ENEMY = 1
}

public class Character : MonoBehaviour
{
    // Settings variables
    public CharacterType characterType = CharacterType.HERO;
    public CharacterStatline statline;

    // Current variable statuses
    public int currentHealth;
    public int remainingMoves;
    public int remainingMeleeAttacks;
    public int remainingRangedAttacks;

    //Move anim parameters (hardcoded)
    public float moveAnimSpeed = 2f;
    public float rotateAnimSpeed = 4f;
    public float moveDestinationTolerance = .1f;
    public float rotateAngleTolerance = .1f;

    // Status variables
    //[HideInInspector] 
    public int xPos;
    //[HideInInspector] 
    public int yPos;
    private Transform cellManagerTransform;
    [HideInInspector] public Cell cell;


    void Awake()
    {
        cellManagerTransform = FindAnyObjectByType<CellManager>().transform;
        currentHealth = statline.health;
        ResetActionAllowances();    
        xPos = (int)transform.position.x;
        yPos = (int)transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        // Update occupied cell connection
        OccupyCell();
    }

    public void OccupyCell()
    {
        // Cell remains the same
        if (cell != null)
        {
            if (cell.x == xPos && cell.y == yPos) return;
        }
        
        // Character changed cells. Clear old cell's occupant. Set new cell occupant and set new cell to cell.
        Transform new_cell = cellManagerTransform.Find("Cell (" + xPos + ", " + yPos + ")");
        if (new_cell == null)
        { 
            cell = null; 
            return; 
        }
        
        if (cell != null) {
            Debug.Log("Changed cells!");
            cell.GetComponent<Cell>().occupant = null; 
        }
        new_cell.GetComponent<Cell>().occupant = this.GetComponent<Character>();
        cell = new_cell.GetComponent<Cell>();
        return;
    }

    public void ResetActionAllowances()
    {
        remainingMoves = statline.moves;
        remainingMeleeAttacks = statline.meleeAttacks;
        remainingRangedAttacks = statline.rangedAttacks;
    }

    //-----------------------------------------------------------------------------------------------------------------//
    //***MOVE ANIMATIONS
    public IEnumerator MoveAlongPath(Cell[] path)
    {
        Cell destination = path[path.Length-1];
        xPos = destination.x; 
        yPos = destination.y;
        this.OccupyCell();

        for(int i = 1; i < path.Length; i++)
        {
            yield return MoveToNextCell(path[i].x, path[i].y);
        }
    }

    private IEnumerator MoveToNextCell(int x, int y)
    {
        yield return RotateAnimation(x, y);
        yield return MoveAnimation(x, y);
    }
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

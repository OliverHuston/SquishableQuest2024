using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterType
{
    HERO = 0,
    ENEMY = 1
}

public class Character : MonoBehaviour
{
    // Start is called before the first frame update
    public int xPos;
    public int yPos;
    public int moveAllowance = 5;

    public CharacterType characterType;
    public Transform cellManagerTransform;
    public Cell cell;


    void Awake()
    {
        cellManagerTransform = FindAnyObjectByType<CellManager>().transform;
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
        //Temp
        xPos = (int)transform.position.x;
        yPos = (int)transform.position.z;

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

        Debug.Log("Changed cells!");
        if (cell != null) cell.GetComponent<Cell>().occupant = null;
        new_cell.GetComponent<Cell>().occupant = this.GetComponent<Character>();
        cell = new_cell.GetComponent<Cell>();
        return;
    }

    public IEnumerator MoveToCell(int x, int y)
    {
        //this.transform.position = new Vector3(x, this.transform.position.y, y);
        yield return MoveAnimation(x, y);
        this.OccupyCell();
    }

    private IEnumerator MoveAnimation(int x, int y)
    {
        Debug.Log("Start Coroutine");
        while(!(transform.position.x == x && transform.position.z == y))
        {
            float moveSpeed = 5f;
            transform.position -= new Vector3(Time.deltaTime * moveSpeed * (transform.position.x - x), 0, Time.deltaTime * moveSpeed * (transform.position.z - y));
            //Increase or decrease the parameter of WaitForSeconds
            //to test different speeds.
            yield return new WaitForSeconds(0.01f);
            Debug.Log("moving...");
        }
        Debug.Log("End Coroutine");
    }
}

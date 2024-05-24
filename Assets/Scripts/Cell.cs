using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellStatus
{
    BASE = 0,
    AVAILABLE = 1,
    SELECTED = 2,
    //TARGETED = 3
}

public class Cell: MonoBehaviour
{
    [HideInInspector] public char cell_code = '0';
    [HideInInspector] public CellStatus status;
    [HideInInspector] public int x;
    [HideInInspector] public int y;
    [HideInInspector] public Room room;
    [HideInInspector] public Character occupant;
    [HideInInspector] public int exitCode = - 1;

    private CellManager cellManager;
    private Material cell_material;
    private bool moused;

    //Colors
    [SerializeField] private Color mousedColor = Color.gray;
    [SerializeField] private Color exitColor = Color.blue;
    [SerializeField] private Color availableColor = Color.green;
    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private Color enemyAvailableColor = Color.red;
    [SerializeField] private Color enemySelectedColor = Color.red;

    private void Awake()
    {
        cellManager = FindAnyObjectByType<CellManager>();
        cell_material = this.gameObject.GetComponent<Renderer>().material;

        this.FindExitCode();


    }

    public void FindExitCode()
    {
        if (cell_code != 'o') { exitCode = -1; return; }

        if(room.exitRooms.Length == 1) { exitCode = 0; }
        else
        {
            for(int i = 0; i < room.exitDirections.Length; i++)
            {
                if (x == room.xPos && room.exitDirections[i] == Direction.WEST)
                {
                    exitCode = i;
                }
                else if (x == room.xPos+room.cols-1 && room.exitDirections[i] == Direction.EAST)
                {
                    exitCode = i;
                }
                else if (y == room.yPos && room.exitDirections[i] == Direction.SOUTH)
                {
                    exitCode = i;
                }
                else if (y == room.yPos + room.rows - 1 && room.exitDirections[i] == Direction.NORTH)
                {
                    exitCode = i;
                }
            }
        }
    }

    private void OnMouseDown()
    {
        cellManager.ReceiveClick(this);
    }

    // Temp color change anim
    private void Update()
    {
        SetCellColor();
    }

    private void SetCellColor()
    {
        if (status == CellStatus.AVAILABLE)
        {
            if (this.occupant == null) { cell_material.SetColor("_BaseColor", availableColor); }
            else { cell_material.SetColor("_BaseColor", enemyAvailableColor); }
        }
        else if (status == CellStatus.SELECTED)
        {
            if (this.occupant == null) { cell_material.SetColor("_BaseColor", selectedColor); }
            else { cell_material.SetColor("_BaseColor", enemySelectedColor); }
        }
        else if (cell_code == 'o' && !room.exitRooms[exitCode].active)
        {
            cell_material.SetColor("_BaseColor", exitColor);
        }
        else
        {
            cell_material.SetColor("_BaseColor", Color.clear);
        }

        // Override color for moused
        /*        if (moused)
                {
                    cell_material.SetColor("_BaseColor", mousedColor);
                }*/
    }


    private void OnMouseEnter()
    {
        moused = true;
    }

    private void OnMouseExit()
    {
        moused = false;
    }
}

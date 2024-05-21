using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public enum SelectionPhase
{
    CHOOSE_CHARACTER = 0,
    ENEMY_CHOSEN = 1,
    HERO_CHOSEN = 2,
    TARGET_SELECTED = 3
}

public class CellManager : MonoBehaviour
{
    private SelectionPhase selectionPhase;
    private Cell originCell;

    // Cell storage array
    private Cell[,] cells;
    private int cellStart_x;
    private int cellStart_y;

    // Convenience references
    MapManager mapManager;

    // Start is called before the first frame update
    void Awake()
    {
        mapManager = FindAnyObjectByType<MapManager>();

        ResetSelection();
    }

    //-----------------------------------------------------------------------------------------------------------------//
    //***CELL CLICK HANDLING
    public void ReceiveClick(Cell source)
    {
        // If click received from Table:
        if (source == null)
        {
            ResetSelection();
            return;
        }

        // Initial character selection.
        if (source.status == CellStatus.BASE && source.occupant != null)
        {
            ResetSelection();

            //Display stats of selected hero or enemy [NEEDS WORK]

            // Selecting a hero (player character):
            if (source.occupant.characterType == CharacterType.HERO)
            {
                DisplayMoveRange(source, source.occupant.moveAllowance, source.occupant.characterType);
                originCell = source;

                selectionPhase = SelectionPhase.HERO_CHOSEN;
                return;
            }
        }
        // Select an available cell.
        else if (source.status == CellStatus.AVAILABLE)
        {
            // If another cell is already selected, reset for selecting new cell.
            if (selectionPhase == SelectionPhase.TARGET_SELECTED)
            {
                SetAllToStatus(CellStatus.BASE);
                DisplayMoveRange(originCell, originCell.occupant.moveAllowance, originCell.occupant.characterType);
            }

            // Select the cell.
            source.status = CellStatus.SELECTED;
            selectionPhase = SelectionPhase.TARGET_SELECTED;
            return;
        }
        // Activate selected cell.
        else if (source.status == CellStatus.SELECTED)
        {
            // Move function (eventually add attack functions; [NEEDS WORK])
            Character character = originCell.occupant;
            StartCoroutine(character.MoveToCell(source.x, source.y));

            if (character.cell.cell_code == 'o')
            {
                mapManager.ExploreRoom(character.cell.room, character.cell.exitCode);
            }

            // Reset and return;
            ResetSelection();
            return;
        }
    }

    //-----------------------------------------------------------------------------------------------------------------//
    //***CELLS ARRAY MANAGEMENT AND ACCESS***
    public void SetCellArrayDimensions(Room[] rooms)
    {
        int minX = 1000; int maxX = -1000;
        int minY = 1000; int maxY = -1000;

        foreach(Room r in rooms) 
        {
            if (r.xPos < minX) minX = r.xPos;
            if(r.xPos + r.cols > maxX) maxX = r.xPos + r.cols;
            if(r.yPos - 1 < minY) minY = r.yPos - 1;
            if (r.yPos + r.rows - 1 > maxY) maxY = r.yPos + r.rows - 1;
        }

        cells = new Cell[maxX - minX + 1, maxY - minY + 1];
        cellStart_x = minX; cellStart_y = minY;
    }
    public void UpdateCellArray()
    {
        foreach(Transform child in transform)
        {
            Cell c = child.gameObject.GetComponent<Cell>(); 
            cells[c.x - cellStart_x, c.y - cellStart_y] = c;
        }
    }
    private Cell FindCell(int x, int y)
    {
        if (x < cellStart_x || y < cellStart_y) return null;
        return cells[x - cellStart_x, y - cellStart_y];
    }

    //-----------------------------------------------------------------------------------------------------------------//
    //***RANGE DISPLAY AND HELPER FUNCTIONS***
    private void DisplayMoveRange(Cell origin, int moveRange, CharacterType characterType)
    {
        TraverseCells(origin, moveRange, characterType);
    }
    private void TraverseCells(Cell origin, int movesRemaining, CharacterType characterType)
    {
        if (movesRemaining <= 0) return;
        for(int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                Cell next_cell = FindCell(origin.x + i, origin.y + j);
                if (!(i == 0 && j == 0) && next_cell != null)
                {
                    if (next_cell.occupant == null)
                    {
                        // Always allow for non-diagonals
                        if(i == 0 || j == 0 || DiagonalAllowed(origin.x, origin.y, i, j, characterType))
                        {
                            next_cell.status = CellStatus.AVAILABLE;
                            TraverseCells(next_cell, movesRemaining - 1, characterType);
                        }
                    }
                }
            }
        }
    }
    private bool DiagonalAllowed(int originX, int originY, int toX, int toY, CharacterType characterType)
    {
        Cell a = FindCell(originX, originY + toY);
        Cell b = FindCell(originX + toX, originY);

        if (a == null || b == null) return false;
        else if (a.occupant == null || b.occupant == null) { return true; }
        return true;
    } 

    //-----------------------------------------------------------------------------------------------------------------//
    //***CELL STATUS SETTERS***
    private void ResetSelection()
    {
        selectionPhase = SelectionPhase.CHOOSE_CHARACTER;
        SetAllToStatus(CellStatus.BASE);
        originCell = null;
    }
    private void SetAllToStatus(CellStatus cellStatus)
    {
        if (cells == null) return;
        foreach (Cell c in cells)
        {
            if (c != null) c.status = cellStatus;
        }
    }
    private void MakeCharactersAvailable()
    {
        if (cells == null) return;
        foreach (Cell c in cells)
        {
            if (c != null) 
            {
                if (c.occupant != null && c.occupant.characterType == CharacterType.HERO) c.status = CellStatus.AVAILABLE;
            }
        }
    }
}

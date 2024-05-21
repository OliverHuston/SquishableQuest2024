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

    // Start is called before the first frame update
    void Awake()
    {
        ResetSelection();
    }

    public void ReceiveClick(Cell source)
    {
        //Debug.Log("Clicked " + source.gameObject.name);
        if (source == null)
        {
            ResetSelection();
            return;
        }

        // Initial character selection.
        if(source.status != CellStatus.AVAILABLE && source.occupant != null)
        {
            // Selecting a character.
            if(source.occupant.characterType == CharacterType.HERO)
            {
                ResetSelection();
                DisplayMoveRange(source, source.occupant.moveAllowance, source.occupant.characterType); //temp hardcode
                originCell = source;
                selectionPhase = SelectionPhase.HERO_CHOSEN;
                return;
            }
            // Selecting an enemy (view stats). [NEEDS WORK]
            else if (source.occupant.characterType == CharacterType.ENEMY) 
            {
                //Display code here....
                return;
            }
        }
        else if(selectionPhase == SelectionPhase.HERO_CHOSEN)
        {
            if(source.status == CellStatus.AVAILABLE)
            {
                source.status = CellStatus.SELECTED;
                selectionPhase = SelectionPhase.TARGET_SELECTED;
                return;
            }
        }
        else if(selectionPhase == SelectionPhase.TARGET_SELECTED)
        {
            if(source.status == CellStatus.SELECTED)
            {
                // Hardcoded move function (needs fixing)
                Character mover = originCell.occupant;
                StartCoroutine(mover.MoveToCell(source.x, source.y));
                

                if (mover.cell.cell_code == 'o')
                {
                    MapManager mapManager = FindAnyObjectByType<MapManager>();
                    mapManager.SetNextRoomActive(mover.cell.room, mover.cell.exitCode);
                }

                ResetSelection();
                return;
            }
            else if(source.status == CellStatus.AVAILABLE)
            {
                ResetSelection();
                DisplayMoveRange(originCell, originCell.occupant.moveAllowance, originCell.occupant.characterType); //temp hardcode
                source.status = CellStatus.SELECTED;
                selectionPhase = SelectionPhase.TARGET_SELECTED;
                return;
            }
        }

        // if nothing else works
        ResetSelection();
        return;
    }

    private void ResetSelection()
    {
        selectionPhase = SelectionPhase.CHOOSE_CHARACTER;
        SetAllToStatus(CellStatus.BASE);
        originCell = null;
    }

    private void MakeCharactersAvailable()
    {
        foreach (Transform child in this.transform)
        {
            Cell cell = child.GetComponent<Cell>();
            if (cell.occupant != null && cell.occupant.characterType == CharacterType.HERO) cell.status = CellStatus.AVAILABLE;
        }
    }

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
                //Transform next_cell_transform = this.transform.Find("Cell (" + (origin.x + i) + ", " + (origin.y + j) + ")");
                if (!(i == 0 && j == 0) && next_cell != null)
                {
                    if (next_cell.occupant == null)
                    {
                        // Always allow for non-diagonals
                        if(i == 0 || j == 0)
                        {
                            next_cell.status = CellStatus.AVAILABLE;
                            TraverseCells(next_cell, movesRemaining - 1, characterType);
                        } 
                        else if (DiagonalAllowed(origin.x, origin.y, i, j, characterType))
                        {
                            next_cell.status = CellStatus.AVAILABLE;
                            TraverseCells(next_cell, movesRemaining - 1, characterType);
                        }
                    }
                }
            }
        }
    }

    private Cell FindCell(int x, int y)
    {
        Transform cell_transform = this.transform.Find("Cell (" + x + ", " + y + ")");
        if (cell_transform == null) return null;

        Cell cell = cell_transform.gameObject.GetComponent<Cell>();
        return cell;
    }

    private bool DiagonalAllowed(int originX, int originY, int toX, int toY, CharacterType characterType)
    {
        Cell a = FindCell(originX, originY + toY);
        Cell b = FindCell(originX + toX, originY);

        if (a == null || b == null) return false;
        //else if (a.occupant == null && b.occupant == null) return true;
        else if(a.occupant == null || b.occupant == null) { return true; }
        
        //if (a.occupant.characterType == characterType && b == null) return true;
        //else if (a == null && b.occupant.characterType == characterType) return true;

        return true;
    }

    private void SetAllToStatus(CellStatus cellStatus)
    {
        foreach (Transform child in this.transform)
        {
            Cell cell = child.GetComponent<Cell>();
            cell.status = cellStatus;
        }
    }
}

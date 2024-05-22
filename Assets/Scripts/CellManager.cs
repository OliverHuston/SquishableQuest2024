using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
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
    private int[,] cellDistances;
    private Cell[] path;

    // Convenience references
    DungeonManager dungeonManager;
    MapManager mapManager;


    // Start is called before the first frame update
    void Awake()
    {
        dungeonManager = FindAnyObjectByType<DungeonManager>(); 
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
                DisplayMoveRange(source);
                originCell = source;

                selectionPhase = SelectionPhase.HERO_CHOSEN;
            }
        }
        // Select an available cell.
        else if (source.status == CellStatus.AVAILABLE)
        {
            // If another cell is already selected, reset for selecting new cell.
            if (selectionPhase == SelectionPhase.TARGET_SELECTED)
            {
                SetAllToStatus(CellStatus.BASE);
                DisplayMoveRange(originCell);
            }

            // Select the cell.
            source.status = CellStatus.SELECTED;
            selectionPhase = SelectionPhase.TARGET_SELECTED;
        }
        // Activate selected cell.
        else if (source.status == CellStatus.SELECTED)
        {
            // Move function (eventually add attack functions; [NEEDS WORK])
            Character character = originCell.occupant;
            int distanceMoved = FindDistance(originCell, source, character.remainingMoves);
            FindPath(originCell, source, distanceMoved);
            PrintPath(); //temp debugging

            character.remainingMoves -= distanceMoved;
            StartCoroutine(character.MoveAlongPath(path));

            if (character.cell.cell_code == 'o')
            {
                mapManager.ExploreRoom(character.cell.room, character.cell.exitCode);
            }

            // Reset and return;
            ResetSelection();
        }

        dungeonManager.CellManagerStatusUpdate(selectionPhase);
    }

    //-----------------------------------------------------------------------------------------------------------------//
    //***ARRAYS MANAGEMENT AND ACCESS***
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
        cellDistances = new int[maxX - minX + 1, maxY - minY + 1];
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
    private int GetCellDistance(int x, int y)
    {
        if (x < cellStart_x || y < cellStart_y) return -100;
        return cellDistances[x - cellStart_x, y - cellStart_y];
    }
    private void SetCellDistance(int x, int y, int distance)
    {
        if (x < cellStart_x || y < cellStart_y) return;
        cellDistances[x - cellStart_x, y - cellStart_y] = distance;
    }
    private void ClearCellDistances()
    {
        for(int i = 0; i < cellDistances.GetLength(0); i++)
        {
            for(int j = 0; j < cellDistances.GetLength(1); j++)
            {
                cellDistances[i, j] = -1;
            }
        }
    }

    //-----------------------------------------------------------------------------------------------------------------//
    //***DISTANCES AND PATHFINDING***
    // Return the distance between two cells; the cells must be within
    private int FindDistance(Cell origin, Cell destination, int searchLength)
    {
        ClearCellDistances();
        TraverseCells(origin, searchLength, MarkDistance);
        FixDistances(searchLength);
        SetCellDistance(origin.x, origin.y, 0);
        return GetCellDistance(destination.x, destination.y);
    }
    // FindDistance helper function (called through TraverseCells).
    private int MarkDistance(Cell c, int movesRemaining)
    {
        if (GetCellDistance(c.x, c.y) == -1 || GetCellDistance(c.x, c.y) < movesRemaining) SetCellDistance(c.x, c.y, movesRemaining);
        return 0;
    }
    // Invert distances to be relative to start position.
    private void FixDistances(int searchLength)
    {
        for (int i = 0; i < cellDistances.GetLength(0); i++)
        {
            for (int j = 0; j < cellDistances.GetLength(1); j++)
            {
                if (cellDistances[i, j] != -1)
                {
                    cellDistances[i, j] = searchLength + 1 - cellDistances[i, j];
                }
            }
        }
    }

    // (NOTE: requires FindDistances to be called first; otherwise, the cellDistances array will be incorrect.)
    private void FindPath(Cell origin, Cell destination, int pathLength )
    {
        path = new Cell[pathLength+1];
        path[0] = origin;
        path[pathLength] = destination;
        TraverseCells(destination, pathLength, AddCellToPath);
    }
    // FindPath helper function (called through TraverseCells).
    private int AddCellToPath(Cell cell, int movesRemaining) {
        if (GetCellDistance(cell.x, cell.y) == -1 || path[movesRemaining] != null) return 0;
        if (GetCellDistance(cell.x, cell.y) == movesRemaining && IsAdjacent(cell, path[movesRemaining + 1]))
        {
            path[movesRemaining] = cell;
        }
        return 0;
    }

    //-----------------------------------------------------------------------------------------------------------------//
    //***RANGE DISPLAY***
    private void DisplayMoveRange(Cell origin)
    {
        TraverseCells(origin, origin.occupant.remainingMoves, SetCellToAvailable);
    }
    private void TraverseCells(Cell origin, int movesRemaining, Func<Cell, int, int> Method)
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
                        if(i == 0 || j == 0 || DiagonalAllowed(origin.x, origin.y, i, j))
                        {
                            TraverseCells(next_cell, movesRemaining - 1, Method);
                            Method(next_cell, movesRemaining);
                        }
                    }
                }
            }
        }
        return;
    }

    //-----------------------------------------------------------------------------------------------------------------//
    //***HELPER FUNCTIONS***
    private bool DiagonalAllowed(int originX, int originY, int toX, int toY)
    {
        if (toX == 0 || toY == 0) return true;
        Cell a = FindCell(originX, originY + toY);
        Cell b = FindCell(originX + toX, originY);

        if (a == null || b == null) return false;
        else if (a.occupant == null || b.occupant == null) { return true; }
        return true;
    }
    private bool IsAdjacent(Cell a, Cell b)
    {
        if (a == null || b == null) return false;
        if (Mathf.Abs(a.x - b.x) <= 1 && Mathf.Abs(a.y - b.y) <= 1 && DiagonalAllowed(a.x, a.y, a.x-b.x, a.y-b.y)) return true;
        return false;
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
    private int SetCellToAvailable(Cell c, int i)
    {
        c.status = CellStatus.AVAILABLE;
        return 0;
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

    //-----------------------------------------------------------------------------------------------------------------//
    //***DEBUGGING***
    private void PrintPath()
    {
        string print = "";
        foreach (Cell c in path)
        {
            if (c != null) print += ("(" + c.x + ", " + c.y + ") -> ");
        }
        Debug.Log(print);
    }
}

using OpenCover.Framework.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SelectionPhase
{
    CHOOSE_CHARACTER = 0,
    ENEMY_CHOSEN = 1,
    HERO_CHOSEN = 2,
    TARGET_SELECTED = 3
}

public class CellManager : MonoBehaviour
{
    // Method adjustment variables
    [Tooltip("The factor by which MoveTowards search is expanded if target is out of move range")]
    public float searchExpansionFactor = 1f;

    //
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
            dungeonManager.CellManagerStatusUpdate(selectionPhase);
            dungeonManager.DisplayCharacterStats(null);

            return;
        }

        // Initial character selection.
        if (source.status == CellStatus.BASE && source.occupant != null)
        {
            ResetSelection();

            // Selecting a hero (player character):
            if (source.occupant.characterType == CharacterType.HERO && source.occupant.available)
            {
                originCell = source;
                DisplayMoveMeleeRanged(originCell);
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
                DisplayMoveMeleeRanged(originCell);
            }

            // Select the cell.
            source.status = CellStatus.SELECTED;
            selectionPhase = SelectionPhase.TARGET_SELECTED;
        }
        // Activate selected cell.
        else if (source.status == CellStatus.SELECTED)
        {
            // Move function
            if (source.occupant == null)
            {
                Character character = originCell.occupant;
                MoveCharacterToCell(character, source);
                dungeonManager.DisplayCharacterStats(character);
            }
            //Attack function
            else
            {
                if (source.occupant.characterType != originCell.occupant.characterType)
                {
                    originCell.occupant.Attack(source.occupant);
                }
                // eventually add further code for special healing abilities etc. that target friendly characters

                dungeonManager.DisplayCharacterStats(originCell.occupant);
            }

            // Reset
            ResetSelection();
        }


        dungeonManager.CellManagerStatusUpdate(selectionPhase);
        if(originCell != null) dungeonManager.DisplayCharacterStats(originCell.occupant);
        //else { dungeonManager.DisplayCharacterStats(null); }
        if(source.occupant != null)
        {
            if (source.occupant.characterType == CharacterType.ENEMY) { dungeonManager.DisplayCharacterStats(source.occupant); }
        }
    }

    private void DisplayMoveMeleeRanged(Cell cell)
    {
        DisplayMoveRange(cell);
        DisplayMeleeTargets(cell);
        DisplayRangedTargets(cell);
    }


    //-----------------------------------------------------------------------------------------------------------------//
    //***ARRAYS MANAGEMENT AND ACCESS***
    public void SetCellArrayDimensions(Room[] rooms)
    {
        int minX = 1000; int maxX = -1000;
        int minY = 1000; int maxY = -1000;

        foreach (Room r in rooms)
        {
            if (r.xPos < minX) minX = r.xPos;
            if (r.xPos + r.cols > maxX) maxX = r.xPos + r.cols;
            if (r.yPos - 1 < minY) minY = r.yPos - 1;
            if (r.yPos + r.rows - 1 > maxY) maxY = r.yPos + r.rows - 1;
        }

        cells = new Cell[maxX - minX + 1, maxY - minY + 1];
        cellDistances = new int[maxX - minX + 1, maxY - minY + 1];
        cellStart_x = minX; cellStart_y = minY;
    }
    public void UpdateCellArray()
    {
        foreach (Transform child in transform)
        {
            Cell c = child.gameObject.GetComponent<Cell>();
            cells[c.x - cellStart_x, c.y - cellStart_y] = c;
        }
    }
    public Cell FindCell(int x, int y)
    {
        if (cells == null) return null;
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
        for (int i = 0; i < cellDistances.GetLength(0); i++)
        {
            for (int j = 0; j < cellDistances.GetLength(1); j++)
            {
                cellDistances[i, j] = -1;
            }
        }
    }

    //-----------------------------------------------------------------------------------------------------------------//
    //***DISTANCES AND PATHFINDING***
    // Multi-purpose function uses for iterating through adjacent and diagonally valid cells. [NEEDS optimization]
    private void TraverseCells(Cell origin, int movesRemaining, Action<Cell, int> Method, bool allDiagonalsAllowed)
    {
        if (movesRemaining <= 0) return;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                Cell next_cell = FindCell(origin.x + i, origin.y + j);
                if (!(i == 0 && j == 0) && next_cell != null)
                {
                    if ((DiagonalAllowed(origin.x, origin.y, i, j) || allDiagonalsAllowed) && next_cell.occupant == null)
                    {
                        Method(next_cell, movesRemaining);
                        TraverseCells(next_cell, movesRemaining - 1, Method, allDiagonalsAllowed);
                    }
                }
            }
        }
        return;
    }
    // Unless explicitly specified, allDiagonalsAllowed will be false.
    private void TraverseCells(Cell origin, int movesRemaining, Action<Cell, int> Method)
    {
        TraverseCells(origin, movesRemaining, Method, false);
    }
    // Return the distance between two cells; the cells must be within searchLength
    private int FindDistance(Cell origin, Cell destination, int searchLength)
    {
        Character removed_occupant = destination.occupant;
        destination.occupant = null; // to avoid issues arising from TraverseCells avoiding occupied cells; maybe there's a more elegant way to do this, but I can't think of one
        ClearCellDistances();
        TraverseCells(origin, searchLength, MarkDistance);
        FixDistances(searchLength);
        SetCellDistance(origin.x, origin.y, 0);
        destination.occupant = removed_occupant;
        return GetCellDistance(destination.x, destination.y);
    }
    // FindDistance helper function (called through TraverseCells).
    private void MarkDistance(Cell cell, int movesRemaining)
    {
        if (GetCellDistance(cell.x, cell.y) == -1 || GetCellDistance(cell.x, cell.y) < movesRemaining) SetCellDistance(cell.x, cell.y, movesRemaining);
        return;
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
    private void FindPath(Cell origin, Cell destination, int pathLength)
    {
        path = new Cell[pathLength + 1];
        path[0] = origin;
        path[pathLength] = destination;
        TraverseCells(destination, pathLength, AddCellToPath);
    }
    // FindPath helper function (called through TraverseCells).
    private void AddCellToPath(Cell cell, int movesRemaining) {
        //Debug.Log("Trying to add (" +cell.x + ", " +cell.y + ") with " + movesRemaining + "moves remaining");
        if (GetCellDistance(cell.x, cell.y) == -1 || movesRemaining == 0 || movesRemaining == path.Length-1 ) { 
            return; 
        }
        else if (GetCellDistance(cell.x, cell.y) == movesRemaining && IsAdjacentByMove(cell, path[movesRemaining + 1]))
        {
            if (path[movesRemaining] != null && IsDiagonal(cell, path[movesRemaining + 1])) return; // prefer straight paths to avoid needless diagonals
            path[movesRemaining] = cell;
            return;
        }
    }
    // Move character to cell; returns false if move is more than allowance.
    public bool MoveCharacterToCell(Character character, Cell destination, int searchRange, bool moveTowardsIfOutOfMoveRange) 
    {
        if(character.remainingMoves == 0) { return false;  }
        int distanceMoved = FindDistance(character.cell, destination, searchRange);
        if(distanceMoved == -1) {
            if (moveTowardsIfOutOfMoveRange) {
                return MoveCharacterToCell(character, destination, searchRange + (int)(character.remainingMoves * searchExpansionFactor), true);
            }
            else { return false; }
        }
        FindPath(character.cell, destination, distanceMoved);

        if (CheckPathIntegrity() == true)
        {
            character.remainingMoves -= distanceMoved;
            StartCoroutine(character.MoveAlongPath(path));
        }
        else
        {
            Debug.Log("ERROR: Incomplete path generated.");
            return false;
        }

        return true;
    }
    public bool MoveCharacterToCell(Character character, Cell destination)
    {
        return MoveCharacterToCell(character, destination, character.remainingMoves, false);
    }
    public bool MoveCharacterToCell(Character character, Cell destination, bool moveTowardsIfOutOfMoveRange)
    {
        return MoveCharacterToCell(character, destination, character.remainingMoves, moveTowardsIfOutOfMoveRange);
    }
    private bool CheckPathIntegrity()
    {
        for(int i = 0; i < path.Length; i++) { if (path[i] == null) return false; }
        return true;
    }

    //-----------------------------------------------------------------------------------------------------------------//
    //***RANGE DISPLAY***
    private void DisplayMoveRange(Cell origin)
    {
        TraverseCells(origin, origin.occupant.remainingMoves, MakeAvailableForMove);
    }
    private void MakeAvailableForMove(Cell cell, int i) 
    {
        cell.status = CellStatus.AVAILABLE;
    }
    private void DisplayMeleeTargets(Cell origin)
    {
        if(origin.occupant.remainingMeleeAttacks <= 0) { return; }
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                Cell next_cell = FindCell(origin.x + i, origin.y + j);
                if (!(i == 0 && j == 0) && next_cell != null)
                {
                    if (next_cell.occupant != null)
                    {
                        if(next_cell.occupant.characterType != origin.occupant.characterType) { next_cell.status = CellStatus.AVAILABLE; }
                    }
                }
            }
        }
    }
    private void DisplayRangedTargets(Cell origin)
    {
        if(origin.occupant.remainingRangedAttacks <= 0) { return; }

        foreach(Character c in dungeonManager.characters)
        {
            if (c != origin.occupant)
            {
                if(LineOfSight(origin, c.cell))
                {
                    if (c.characterType != origin.occupant.characterType && !IsAdjacent(origin, c.cell)) { c.cell.status = CellStatus.AVAILABLE; }
                }
            }
        }
    }
    public bool LineOfSight(Cell a, Cell b)
    {
        if(a == null || b == null) {return false;}

        float m = 10000f;
        if (a.x != b.x) m = (float)(a.y - b.y) / (a.x - b.x);
        float c = - (a.x * m) + a.y;

        for (int i = Mathf.Min(a.x, b.x); i < Mathf.Max(a.x, b.x) + 1; i++)
        {
            for (int j = Mathf.Min(a.y, b.y); j < Mathf.Max(a.y, b.y) + 1; j++)
            {
                
                float x_min = i - .5f;   float x_max = i + .5f;
                float y_min = j - .5f;   float y_max = j + .5f;

                if ((m > 0 && m * x_min + c <= y_max && m * x_max + c >= y_min) 
                    || (m <= 0 && m * x_min + c >= y_min && m * x_max + c <= y_max))
                {
                    //Debug.Log("* " + i + ", " + j);
                    if (FindCell(i, j) == null) return false;
                }
                else
                {
                    //Debug.Log(i + ", " + j);
                }  
            }
        }

        return true;
    }

    //-----------------------------------------------------------------------------------------------------------------//
    //***HELPER FUNCTIONS***
    private bool DiagonalAllowed(int originX, int originY, int toX, int toY)
    {
        if (toX == 0 || toY == 0) { return true; }
        else if (toX < -1 || toY < -1 || toX > 1 || toY > 1) { return false; }

        Cell a = FindCell(originX, originY + toY);
        Cell b = FindCell(originX + toX, originY);

        if (a == null || b == null) { return false; }
        else if (a.occupant == null || b.occupant == null) { return true; }
        return false;
    }
    private bool IsDiagonal(Cell a, Cell b)
    {
        if (a == null || b == null) return false;
        if (a.x == b.x || a.y == b.y) return false;
        return true;
    }
    private bool IsAdjacentByMove(Cell a, Cell b)
    {
        if (a == null || b == null) return false;
        if (DiagonalAllowed(a.x, a.y, b.x-a.x, b.y-a.y)) return true;
        return false;
    }
    public bool IsAdjacent(Cell a, Cell b)
    {
        int toX = a.x - b.x;
        int toY = a.y - b.y;
        if (toX < -1 || toY < -1 || toX > 1 || toY > 1) { return false; }
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

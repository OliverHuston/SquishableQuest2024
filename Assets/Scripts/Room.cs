using UnityEngine;
using System.Linq;

public enum Direction
{
    NA = -1,
    NORTH = 0,
    EAST = 1,
    SOUTH = 2,
    WEST = 3
}

public enum RoomType
{
    STANDARD = 0,
    START = 1,
    TWO_BRANCH = 2,
    THREE_BRANCH = 3,
    DEADEND = 4,
    BOSS = 5,
    SPECIAL = 6
}

[CreateAssetMenu]
public class Room : ScriptableObject
{
    // Exposed Settings
    public string _Name;
    public RoomType _Type;
    public GameObject _roomModel;
    [TextAreaAttribute]
    public string layout;

    // Dimensions and Position
    [HideInInspector] public int cols;
    [HideInInspector] public int rows;
    [HideInInspector] public int xPos;
    [HideInInspector] public int yPos;

    // Entrance
    [HideInInspector] public Direction entranceDirection;
    [HideInInspector] public int entranceCoord;

    // Exits
    [HideInInspector] public Direction[] exitDirections;
    [HideInInspector] public int[] exitCoords;
    [HideInInspector] public Room[] exitRooms;

    // Other
    [HideInInspector] public bool active = false;
    private char[,] cells;
    public char[,] getCells()
    {
        return this.cells;
    }


    //-----------------------------------------------------------------------------------------------------------------//
    //***SETUP***
    public void Initialize()
    {
        // 1. Get dimensions and count exits.
        cols = 0;   rows = 1;
        int exit_count = 0;
        for(int i = 0; i < layout.Length; i++)
        {
            if (layout[i] == '\n') { rows++; }
            else if (rows == 1) { 
                cols++;
            }
            if (layout[i] == 'o') exit_count++;
        }
        exitDirections = new Direction[exit_count / 2];
        exitRooms = new Room[exit_count / 2];
        exitCoords = new int[exit_count / 2];

        // 2. Fill in cells array.
        cells = new char[cols, rows];
        int count = 0;
        for (int i = 0; i < layout.Length; i++)
        {
            if (layout[i] != '\n') {
                cells[count % cols, count / cols] = layout[i];
                count++;
            }
        }

        // 3. Set remaining variables.
        UpdateEntrancesAndExits();
        xPos = 0;
        yPos = 0;
        active = false;
    }

    //-----------------------------------------------------------------------------------------------------------------//
    //***POSITIONING***
    // Rotate clockwise. Origin stays in the top right (in current camera setup).
    public void Rotate()
    {
        int newCols = rows;
        int newRows = cols;

        char[,] newCells = new char[newCols, newRows];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                newCells[i, j] = cells[cols - j - 1, i];
            }
        }

        cells = newCells;
        rows = newRows;
        cols = newCols;
        UpdateEntrancesAndExits();
    }

    //-----------------------------------------------------------------------------------------------------------------//
    //***ENTRANCE FINDING***
    // Set correct entrance and exit directions.
    private void UpdateEntrancesAndExits()
    {
        entranceDirection = FindEntranceDirection();
        exitDirections = FindExitDirections();
    }
    // Returns entrance direction; a room should have only a single entrance direction.
    private Direction FindEntranceDirection()
    {
        char find_char = 'i';

        int col1 = -1;  int row1 = -1;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (cells[j, i] == find_char)
                {
                    col1 = j; row1 = i;
                    goto exitloop;
                }
            }
        }
        //if (col1 == -1 && row1 == -1) return Direction.NA;
        exitloop:
        Direction d = DirectionFromOneOfPair(col1, row1);
        if (d == Direction.NORTH || d == Direction.SOUTH) { entranceCoord = col1; }
        else if (d == Direction.WEST || d == Direction.EAST) { entranceCoord = rows - 2 - row1; }

        return d;
    }
    // Return a direction array of all exit directions. (***Only allows one exit per direction.)
    private Direction[] FindExitDirections()
    {
        char find_char = 'o';
        int count = 0;
        Direction[] result = new Direction[exitDirections.Length];
        for(int i = 0; i < exitDirections.Length; i++)
        {
            result[i] = Direction.NA;
        }

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (cells[j, i] == find_char)
                {
                    Direction d = DirectionFromOneOfPair(j, i);
                    if (!result.Contains(d))
                    {
                        //Debug.Log(this.Name + " USING " + j + ", " + i);
                        result[count] = d;
                        if (d == Direction.NORTH || d == Direction.SOUTH) { exitCoords[count] = j; }
                        else if (d == Direction.WEST || d == Direction.EAST) { exitCoords[count] = rows - 2 - i; }
                        count++;
                    }
                }
            }
        }
        
        return result;
    }
    // Helper function for finding entrance and exit directions.
    private Direction DirectionFromOneOfPair(int col1, int row1)
    {
        int col2 = -1; int row2 = -1;
        for (int i = Mathf.Max(0, row1-1); i < Mathf.Min(row1+2, rows); i++)
        {
            for (int j = Mathf.Max(0, col1-1); j < Mathf.Min(col1+2, cols); j++)
            {
                if (cells[j, i] == cells[col1, row1] && !(col1 == j && row1 == i))
                {
                    col2 = j;
                    row2 = i;
                }
            }
        }

        if (col2 == col1)
        {
            if (col1 == 0) return Direction.WEST;
            else { return Direction.EAST; }
        }
        else if (row2 == row1)
        {
            if (row1 == 0) return Direction.NORTH;
            else { return Direction.SOUTH; }
        }

        return Direction.NA;
    }

    //-----------------------------------------------------------------------------------------------------------------//
    //***DEBUGGING FUNCTIONS***
    // Print contents of cells array to debug log.
    private void PrintCellsToLog()
    {
        for (int i = 0; i < rows; i++)
        {
            string line = "";
            for (int j = 0; j < cols; j++)
            {
                line = line + cells[j, i];
            }
            Debug.Log(line);
        }
    }
}
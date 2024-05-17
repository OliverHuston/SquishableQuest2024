using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    // Library and prefabs
    public Room[] roomsLibrary;

    public GameObject _cellPrefab;
    public GameObject _placeholderPrefab;

    // Private variables
    private Room[] rooms;
    private int roomsCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            int iterations = GenerateRandomMap(20, 3, 0);
            rooms[0].active = true;
            PrintRoom(rooms[0]);

            //SetAllRoomsActive();
            //PrintFullMap();
            //Debug.Log("Generated in " + iterations + " iterations.");
        }
    }

    private void SetAllRoomsActive()
    {
        foreach (Room r in rooms)
        {
            if (r != null) r.active = true;
        }
    }

    public void SetNextRoomActive(Room currentRoom, int exitNum)
    {
        if( currentRoom.exitRooms[exitNum].active == false)
        {
            currentRoom.exitRooms[exitNum].active = true;
            PrintRoom(currentRoom.exitRooms[exitNum]);
        }
    }


    //-----------------------------------------------------------------------------------------------------------------//
    //***MAP GEN FUNCTIONS***
    // Generates a valid map randomly based on given parameters. Returns int number of iterations it took to generate.
    private int GenerateRandomMap(int numRooms, int numTwoBranch, int numThreeBranch)
    {
        ClearMap();
        GenerateMapIteration(numRooms, numTwoBranch, numThreeBranch);
        int iterations = 1;
        while (!CheckMapIntegrity())
        {
            ClearMap();
            GenerateMapIteration(numRooms, numTwoBranch, numThreeBranch);
            iterations++;
        }
        return iterations;
    }
    // Attempt to generate a map using the given parameters. Map may not be valid.
    private void GenerateMapIteration(int numRooms, int numTwoBranch, int numThreeBranch)
    {
        if (numRooms - 1 < MinimumRoomRequirement(numTwoBranch, numThreeBranch))
        {
            Debug.Log("ERROR: Alloted number of rooms is too small to create this configuration.");
            return;
        }
        roomsCount = 0;
        rooms = new Room[numRooms];
        Room firstRoom = AddRoom(ChooseRoom("start"), 0, null);

        GeneratePath(firstRoom, numRooms - 1, 0, true, numTwoBranch, numThreeBranch);
    }
    // Recursively called to generate a series of StraightPaths containing branches.
    private void GeneratePath(Room startRoom, int pathLength, int exitPath, bool bossPath, int numTwoBranch, int numThreeBranch)
    {
        //Debug.Log(bossPath + " - Generating path from " + startRoom._Name + " with len " + pathLength + ", containg " + numTwoBranch + " 2-branches & " + numThreeBranch + " 3-branches");

        //if (pathLength <= 0) { return; }
        if (numTwoBranch == 0) {
            if (bossPath == true) { GenerateStraightPath(pathLength, startRoom, exitPath, "boss"); }
            else if (bossPath == false) { GenerateStraightPath(pathLength, startRoom, exitPath, "deadend"); }
        }
        else if (numTwoBranch > 0)
        {
            // Create straight path to first branch
            int pathToSplitLength = RandomInt(1, pathLength - MinimumRoomRequirement(numTwoBranch, numThreeBranch) + 1);

            pathLength = pathLength - pathToSplitLength;
            numTwoBranch = numTwoBranch - 1;

            // Recursively call to fill in splits
            int numTwoBranchPartition = RandomInt(0, numTwoBranch);
            int numThreeBranchPartition = RandomInt(0, numThreeBranch);

            int pathLengthPartition = RandomInt(MinimumRoomRequirement(numTwoBranch, numThreeBranch),
                pathLength - MinimumRoomRequirement(numTwoBranch - numTwoBranchPartition, numThreeBranch - numThreeBranchPartition));

            bool isBossPathA = RandomBool();
            bool isBossPathB = !isBossPathA;
            if(!bossPath) { isBossPathA = false; isBossPathB = false; }


            Room lastRoom = GenerateStraightPath(pathToSplitLength, startRoom, exitPath, "two_branch");
            //Debug.Log(pathToSplitLength + "  " + pathLengthPartition + "  " + (pathLength-pathLengthPartition));

            GeneratePath(lastRoom, pathLengthPartition, 0, isBossPathA, numTwoBranchPartition, numThreeBranchPartition);
            GeneratePath(lastRoom, pathLength - pathLengthPartition, 1, isBossPathB, numTwoBranch - numTwoBranchPartition, numThreeBranch - numThreeBranchPartition);
        }
    }
    // Generate a path of STANDARD rooms beginning from startRoom and exit exitPath; the final room in the path is of type terminus_type.
    private Room GenerateStraightPath(int pathLength, Room startRoom, int exitPath, string terminus_type)
    {
        if(startRoom == null) { return null; }
        else if (pathLength < 1)
        {
            //Debug.Log("ERROR: Cannot generate a path with length less than 1.");
            return null;
        }

        // Add standard rooms.
        for (int i = 0; i < pathLength - 1; i++) { AddRoom(ChooseRoom("standard"), exitPath, startRoom); }

        // Add last room and return it.
        Room lastRoom = AddRoom(ChooseRoom(terminus_type), exitPath, startRoom);
        return lastRoom;
    }
    // Add a room to a currentRoom from the provided exitnum.
    private Room AddRoom(int room_code, int exit_num, Room currentRoom)
    {
        Room newRoom = Instantiate(roomsLibrary[room_code]);
        newRoom.Initialize();

        // For placing first room. currentRoom should be entered as null
        if (currentRoom == null && roomsCount == 0)
        {
            PlaceNextRoom(null, exit_num, newRoom);
            rooms[roomsCount] = newRoom;
            roomsCount++;
            return rooms[0];
        }

        while (AvailableExit(currentRoom.exitRooms, exit_num) != null)
        {
            currentRoom = AvailableExit(currentRoom.exitRooms, exit_num);
            if (currentRoom.exitRooms.Length == 0) { return null; }
        }

        if (roomsCount >= rooms.Length) { 
            //Debug.Log("!!! tried to place a room at index of " + roomsCount);
            return null; 
        }

        PlaceNextRoom(currentRoom, AvailableExitIndex(currentRoom.exitRooms, exit_num), newRoom);
        currentRoom.exitRooms[AvailableExitIndex(currentRoom.exitRooms, exit_num)] = newRoom;
        rooms[roomsCount] = newRoom;
        roomsCount++;
        return newRoom;
    }
    // Place newRoom, aligning its entrance with a given exit of currentRoom.
    private void PlaceNextRoom(Room currentRoom, int exitIndex, Room newRoom)
    {
        if (currentRoom == null)
        {
            newRoom.xPos = 0; newRoom.yPos = 0;
            return;
        }

        Direction exitDir = currentRoom.exitDirections[exitIndex];
        while (((int)newRoom.entranceDirection + 2) % 4 != (int)exitDir)
        {
            newRoom.Rotate();
        }

        switch (exitDir)
        {
            case Direction.NORTH:
                newRoom.yPos = currentRoom.yPos + currentRoom.rows;
                newRoom.xPos = currentRoom.xPos + currentRoom.exitCoords[exitIndex] - newRoom.entranceCoord;
                break;
            case Direction.SOUTH:
                newRoom.yPos = currentRoom.yPos - newRoom.rows;
                newRoom.xPos = currentRoom.xPos + currentRoom.exitCoords[exitIndex] - newRoom.entranceCoord;
                break;
            case Direction.EAST:
                newRoom.xPos = currentRoom.xPos + currentRoom.cols;
                newRoom.yPos = currentRoom.yPos + currentRoom.exitCoords[exitIndex] - newRoom.entranceCoord;
                break;
            case Direction.WEST:
                newRoom.xPos = currentRoom.xPos - newRoom.cols;
                newRoom.yPos = currentRoom.yPos + currentRoom.exitCoords[exitIndex] - newRoom.entranceCoord;
                break;
        }
    }
    // Reset rooms & roomCount. Destroy all cells and map models.
    private void ClearMap()
    {
        rooms = null;
        roomsCount = 0;
        foreach (Transform child in this.transform.GetChild(0).transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in this.transform.GetChild(1).transform)
        {
            Destroy(child.gameObject);
        }
    }

    //-----------------------------------------------------------------------------------------------------------------//
    //***MAP GEN HELPER FUNCTIONS***
    // Calculate the minimum number of rooms required for a path containing a given number of 2- and 3-branches.
    private int MinimumRoomRequirement(int numTwoBranch, int numThreeBranch)
    {
        // if no branches -> min = 1
        // if 1 2-branch, min = 3
        // if 1 3-branch, min = 4
        // if 2 2-branch, min = 3 + 2 = 5
        // if 3 3-branch, min = 3 + 2 + 2 = 7
        // if 1 of each, min = 3 + 3 or 4 + 2
        // if 2 2-br & 1 3-br, min = 3 + 2 + 3 or 4 + 2 + 2 = 8
        // if 1 2-br & 2 3-br, min = 3 + 3 + 3 or 4 + 2 + 3 = 8
        return 1 + (2 * numTwoBranch) + (3 * numThreeBranch);
    }
    // Helper functions to find best room path within array bounds.
    private Room AvailableExit(Room[] exits, int intended_index)
    {
        if(intended_index <= exits.Length-1) { return exits[intended_index]; }
        else { return exits[exits.Length-1];}
    }
    private int AvailableExitIndex(Room[] exits, int intended_index)
    {
        if (intended_index <= exits.Length - 1) { return intended_index; }
        else { return exits.Length - 1; }
    }
    // Choose random room based on type specified in string mode.
    private int ChooseRoom(string mode)
    {
        // Assign target_type based on string mode.
        RoomType target_type = RoomType.STANDARD;
        if (mode.Equals("standard")) { target_type = RoomType.STANDARD; }
        else if (mode.Equals("start")) { target_type = RoomType.START; }
        else if (mode.Equals("boss")) { target_type = RoomType.BOSS; }
        else if (mode.Equals("two_branch")) { target_type = RoomType.TWO_BRANCH; }
        else if (mode.Equals("three_branch")) { target_type = RoomType.THREE_BRANCH; }
        else if (mode.Equals("deadend")) { target_type = RoomType.DEADEND; }

        // Randomly select a room from the roomsLibrary until a room of the correct type is chosen.
        int room_code = (int)Random.Range(0, roomsLibrary.Length);
        while (roomsLibrary[room_code]._Type != target_type)
        {
            room_code = (int)Random.Range(0, roomsLibrary.Length);
        }
        return room_code;
    }

    //-----------------------------------------------------------------------------------------------------------------//
    //***MAP INTEGRITY CHECKING***
    private bool CheckMapIntegrity()
    {
        // Ensure no overlap
        for (int i = 0; i < rooms.Length; i++)
        {
            for (int j = 0; j < rooms.Length; j++)
            {
                if (j != i)
                {
                    if (rooms[i] == null || rooms[j] == null) { return false; }
                    else if (RoomOverlap(rooms[i], rooms[j]))
                    {
                        return false;
                    }
                }
            }
        }

        // Ensure all paths are complete
        for (int i = 0; i < rooms.Length; i++)
        {
            foreach (Room exit in rooms[i].exitRooms)
            {
                if (exit == null) { return false; }
            }
        }

        return true;
    }
    // Returns true if rooms a & b overlap, false if not.
    private bool RoomOverlap(Room a, Room b)
    {
        // Allow connecting rooms to abut each other without 1 tile buffer.
        if (RoomsConnecting(a, b)) { return false; }

        // Make sure non-connecting rooms do not overlap and have 1 tile buffer.
        else if (a.xPos > b.xPos + b.cols
            || a.xPos + a.cols < b.xPos
            || a.yPos > b.yPos + b.rows
            || a.yPos + a.rows < b.yPos)
        {
            return false;
        }

        return true;
    }
    // Returns true if rooms a & b are connected, false if not.
    private bool RoomsConnecting(Room a, Room b)
    {
        foreach (Room r in a.exitRooms)
        {
            if (r == b) return true;
        }
        foreach (Room r in b.exitRooms)
        {
            if (r == a) return true;
        }
        return false;
    }

    //-----------------------------------------------------------------------------------------------------------------//
    //***MAP DISPLAY FUNCTIONS***
    // Print full map (active rooms only).
    private void PrintFullMap()
    {
        foreach (Room r in rooms)
        {
            if (r != null) PrintRoom(r);
        }
    }
    // Display a single room and create cells for it.
    private void PrintRoom(Room room)
    {
        if (room.active == false) return;

        if (room._roomModel == null) PrintRoomPlaceholder(room);
        //else {PrintRoomModel(room);}
        //Debug.Log("a");
        PrintRoomCells(room);
    }
    // Instantiate cells for a given room.
    private void PrintRoomCells(Room room)
    {
        
        int cols = room.cols;
        int rows = room.rows;
        char[,] cells = room.getCells();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (cells[j, i] != '0')
                {
                    GameObject newCell = Instantiate(_cellPrefab, transform.position, Quaternion.identity);
                    newCell.name = "Cell (" + (room.xPos + j) + ", " + (room.yPos + rows - 1 - i) + ")";
                    newCell.transform.parent = this.transform.GetChild(0);
                    newCell.transform.position += new Vector3(room.xPos + j, 0, room.yPos + rows - 1 - i);
                    newCell.GetComponent<Cell>().x = room.xPos + j;
                    newCell.GetComponent<Cell>().y = room.yPos + rows - 1 - i;
                    newCell.GetComponent<Cell>().cell_code = cells[j, i];
                    newCell.GetComponent<Cell>().room = room;
                    newCell.GetComponent<Cell>().FindExitCode();
                }
            }
        }
    }
    // Create a placeholder model for a given room.
    private void PrintRoomPlaceholder(Room room)
    {
        GameObject roomPlaceholder = new GameObject("Placeholder: " + room._Name);
        roomPlaceholder.transform.parent = this.transform.GetChild(1);
        roomPlaceholder.transform.position += new Vector3(room.xPos, 0, room.yPos);

        int cols = room.cols;
        int rows = room.rows;
        char[,] cells = room.getCells();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (cells[j, i] != '0')
                {
                    GameObject newCube = Instantiate(_placeholderPrefab, transform.position, Quaternion.identity);
                    newCube.transform.parent = roomPlaceholder.transform;
                    newCube.transform.position += new Vector3(room.xPos + j, 0, room.yPos + rows - 1 - i);
                }
                //temp debugging
                /*                GameObject a = Instantiate(_placeholderPrefab, transform.position, Quaternion.identity);
                                a.transform.parent = roomPlaceholder.transform;
                                a.transform.position += new Vector3(room.xPos + j, 0, room.yPos + rows - 1 - i);*/
            }
        }
    }
    // Create 3d model for room. //NEEDS WORK
    private void PrintRoomModel(Room room)
    {

    }

    //-----------------------------------------------------------------------------------------------------------------//
    //***OTHER HELPER FUNCTIONS***
    // Return a random bool.
    private bool RandomBool()
    {
        return (Random.value > 0.5f);
    }
    // Return an int between two values a and b (inclusive). a & b do not have to be in order.
    private int RandomInt(int a, int b)
    {
        if (a > b) { return Random.Range(b, a + 1); }
        else if (a == b) { return a; }
        return Random.Range(a, b+1);
    }
}
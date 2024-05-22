using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using UnityEngine;

public enum Turn
{
    PLAYER = 0,
    ENEMIES = 1,
    POPUP = 2
}
   

public class DungeonManager : MonoBehaviour
{
    public Turn turn;

    public GameObject endTurnButton;

    private Character[] heroes;
    private Character[] enemies;

    // Start is called before the first frame update
    void Awake()
    {
        heroes = FindObjectsOfType<Character>();  //needs fixing to exclude enemies
        endTurnButton.SetActive(false);
    }

    public void EndTurn()
    {
        endTurnButton.SetActive(false);
        turn = Turn.ENEMIES;
        EnemyTurn();
    }

    public void CellManagerStatusUpdate(SelectionPhase selectionPhase)
    {
        if(selectionPhase == SelectionPhase.CHOOSE_CHARACTER) endTurnButton.SetActive(true);
        else { endTurnButton.SetActive(false); }
    }

    private void EnemyTurn()
    {
        //work here

        turn = Turn.PLAYER;
        StartPlayerTurn();
        return;
    }

    private void StartPlayerTurn()
    {
        foreach (Character c in heroes)
        {
            c.ResetActionAllowances();
        }
    }
}

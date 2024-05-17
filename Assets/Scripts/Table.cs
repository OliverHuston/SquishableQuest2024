using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    private CellManager cellManager;

    private void Awake()
    {
        cellManager = FindAnyObjectByType<CellManager>();
    }

    private void OnMouseDown()
    {
        cellManager.ReceiveClick(null);
    }
}

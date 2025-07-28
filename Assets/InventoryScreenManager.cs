using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryScreenManager : MonoBehaviour
{
    public ItemDisplay draggedItemDisplay;

    public static InventoryScreenManager instance { get; private set; }
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one InventoryScreenManager in the scene.");
        }
        instance = this;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}

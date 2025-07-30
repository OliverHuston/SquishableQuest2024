using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryScreenManager : MonoBehaviour
{
    public ItemDisplay draggedItemDisplay;
    public InventorySlot highlightedSlot;

    [SerializeField] private Transform characterInventory;
    [SerializeField] private Transform storageInventory;

    private InventorySlot[] characterInventorySlots;
    private InventorySlot[] storageInventorySlots;



    public static InventoryScreenManager instance { get; private set; }
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one InventoryScreenManager in the scene.");
        }
        instance = this;

        characterInventorySlots = characterInventory.GetComponentsInChildren<InventorySlot>();
        storageInventorySlots = storageInventory.GetComponentsInChildren<InventorySlot>();
    }



    public InventorySlot FindAvailableStorageSlot()
    {
        foreach(InventorySlot inventorySlot in storageInventorySlots)
        {
            if(inventorySlot.CheckIfOccupied() == false) return inventorySlot;
        }

        Debug.Log("Something went wrong. Could not find an available Inventory Slot in the Storage Inventory.");
        return null;
    }

    public InventorySlot FindNearestAvailableStorageSlot(Vector3 position)
    {
        float closestPossibleDistance = 112.0f; //hardcoded minimum distance based on layout --- should save time in this operation

        InventorySlot closestAvailableSlot = null;
        float closestDistance = 10000;

        foreach (InventorySlot inventorySlot in storageInventorySlots)
        {
            float distance = Vector3.Distance(inventorySlot.transform.position, position);
            if (inventorySlot.CheckIfOccupied() == false && distance < closestDistance) {
                closestDistance = distance;
                closestAvailableSlot = inventorySlot;
                if (closestPossibleDistance >= closestDistance) return closestAvailableSlot;
            }
        }

        return closestAvailableSlot;
    }


    //(closest possible distance = 110)
}

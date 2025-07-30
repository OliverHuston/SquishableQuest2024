using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ItemRarity restrictedRarity = ItemRarity.NA;
    [Space]
    
    [SerializeField] private Color regularColor;
    [SerializeField] private Color activeColor;

    public ItemDisplay slottedItemDisplay;
    private Image backdrop;

    void Awake()
    {
        backdrop = this.transform.GetChild(0).GetComponent<Image>();
        backdrop.color = regularColor;
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (InventoryScreenManager.instance.draggedItemDisplay != null)
        {
            if(ItemCompatible(InventoryScreenManager.instance.draggedItemDisplay))
            {
                backdrop.color = activeColor;
                InventoryScreenManager.instance.highlightedSlot = this;
            }
        }
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        backdrop.color = regularColor;
        InventoryScreenManager.instance.highlightedSlot = null;
    }

    public bool CheckIfOccupied()
    {
        if (this.slottedItemDisplay != null) return true;
        return false;
    }

    public bool ItemCompatible(ItemDisplay itemDisplay)
    {
        if (itemDisplay == null) return false;
        Item item = itemDisplay.item;
        if (item == null) return false;
        if(this.restrictedRarity == ItemRarity.NA) { return true; }
        else if(this.restrictedRarity ==  item.rarity) { return true; }
        return false;
    }
}

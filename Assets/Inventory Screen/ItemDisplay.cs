using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ItemDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Item item;

    [Space]
    [SerializeField] Color commonColor;
    [SerializeField] Color uncommonColor;
    [SerializeField] Color rareColor;

    [Space]
    [SerializeField] private float scaleIncreaseOnMoused = 1.10f;
    [SerializeField] private float scaleIncreaseOnClicked = 1.20f;

    [Space]
    public InventorySlot slot;


    private Vector3 baseScale;
    private bool dragActive = false;

    private Image backdrop;
    private Image itemDisplayImage;

    void Awake()
    {
        baseScale = this.transform.localScale;
        backdrop = this.transform.GetChild(0).GetComponent<Image>();
        itemDisplayImage = this.transform.GetChild(3).GetComponent<Image>();

        this.SetAppearance();
        this.PlaceInSlot(this, slot);
    }

    void Update()
    {
        if(dragActive)
        {
            this.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            this.transform.SetAsLastSibling();
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if(this.slot.ItemCompatible(InventoryScreenManager.instance.draggedItemDisplay))
        {
            InventoryScreenManager.instance.highlightedSlot = this.slot;
            this.transform.localScale = baseScale * scaleIncreaseOnMoused;
        }
        else if(InventoryScreenManager.instance.draggedItemDisplay == null)
        {
            this.transform.localScale = baseScale * scaleIncreaseOnMoused;
        }
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if(this.dragActive == false) this.transform.localScale = baseScale;
        InventoryScreenManager.instance.highlightedSlot = null;
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        this.transform.localScale = baseScale * scaleIncreaseOnClicked;
        this.dragActive = true;
        InventoryScreenManager.instance.draggedItemDisplay = this;
        this.GetComponent<Image>().raycastTarget = false;
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        this.transform.localScale = baseScale * scaleIncreaseOnMoused;
        this.dragActive = false;
        this.GetComponent<Image>().raycastTarget = true;

        if (InventoryScreenManager.instance.highlightedSlot == null)
        {
            this.PlaceInSlot(this, slot);
        }
        else
        {
            this.PlaceInSlot(this, InventoryScreenManager.instance.highlightedSlot);
        }
        InventoryScreenManager.instance.draggedItemDisplay = null;

    }

    private void SetAppearance()
    {
        if (item == null) return;

        // Backdrop Color
        if(item.rarity == ItemRarity.COMMON)
        {
            backdrop.color = this.commonColor;
        }
        else if (item.rarity == ItemRarity.UNCOMMON)
        {
            backdrop.color = this.uncommonColor;
        }
        else if (item.rarity == ItemRarity.RARE)
        {
            backdrop.color = this.rareColor;
        }

        // Display PNG
        if(item.displayPNG != null)
        {
            itemDisplayImage.sprite = item.displayPNG;
        }
    }

    public void PlaceInSlot(ItemDisplay itemDisplay, InventorySlot targetSlot)
    {
        InventorySlot currentSlot = itemDisplay.slot;
        ItemDisplay itemSlottedInTarget = targetSlot.slottedItemDisplay;

        if (targetSlot != currentSlot && itemSlottedInTarget != null)
        {
            // If the items can swap
            if(currentSlot.ItemCompatible(itemSlottedInTarget))
            {
                itemSlottedInTarget.transform.position = currentSlot.transform.position;
                currentSlot.slottedItemDisplay = itemSlottedInTarget;
                itemSlottedInTarget.slot = currentSlot;
            }
            // If the items can't swap...
            else
            {
                currentSlot.slottedItemDisplay = null;
                InventorySlot newSlot = InventoryScreenManager.instance.FindNearestAvailableStorageSlot(targetSlot.transform.position);

                itemSlottedInTarget.transform.position = newSlot.transform.position;
                newSlot.slottedItemDisplay = itemSlottedInTarget;
                itemSlottedInTarget.slot = newSlot;
            }
        }
        else
        {
            if (currentSlot != null) currentSlot.slottedItemDisplay = null;
        }

        itemDisplay.transform.position = targetSlot.transform.position;
        targetSlot.slottedItemDisplay = itemDisplay;
        itemDisplay.slot = targetSlot;


    }
}

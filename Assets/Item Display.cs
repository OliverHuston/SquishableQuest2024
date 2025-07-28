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

        this.transform.position = slot.transform.position;

        backdrop = this.transform.GetChild(0).GetComponent<Image>();
        itemDisplayImage = this.transform.GetChild(3).GetComponent<Image>();

        this.UpdateAppearance();
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
        this.transform.localScale = baseScale * scaleIncreaseOnMoused;
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        this.transform.localScale = baseScale;

    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        this.transform.localScale = baseScale * scaleIncreaseOnClicked;
        //this.startPos = this.transform.position;
        this.dragActive = true;
        InventoryScreenManager.instance.draggedItemDisplay = this;
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        this.transform.localScale = baseScale * scaleIncreaseOnMoused;
        this.transform.position = slot.transform.position;
        this.dragActive = false;
        InventoryScreenManager.instance.draggedItemDisplay = null;
    }

    private void UpdateAppearance()
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
}

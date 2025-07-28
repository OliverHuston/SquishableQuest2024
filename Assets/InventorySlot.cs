using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Color regularColor;
    [SerializeField] private Color activeColor;

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
            backdrop.color = activeColor;
        }
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        backdrop.color = regularColor;

    }
}

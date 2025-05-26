using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Province : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RegionalData regionalData;

    [Space]
    [SerializeField] private Color unavailable_color = Color.clear;
    [SerializeField] private Color locked_color = Color.clear;
    [SerializeField] private Color unlocked_color = Color.clear;
    [SerializeField] private Color moused_color = Color.white;

    private Image image;

    void Awake()
    {
        this.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
        image = this.GetComponent<Image>();

        this.SetColor();
        if (EventSystem.current.IsPointerOverGameObject()) highlightProvince();
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        highlightProvince();
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        unhighlightProvince();
    }

    private void SetColor()
    {
        if(this.regionalData.unlocked_status == RegionUnlocked.UNLOCKED) { image.color = unlocked_color; }
        else if (this.regionalData.unlocked_status == RegionUnlocked.LOCKED) { image.color = locked_color; }
        else { image.color = unavailable_color; }
    }

    private void highlightProvince()
    {
        image.color = moused_color;
        ProvinceManager.instance.DisplayProvinceDetails(this.regionalData.region_name, this.regionalData.region_description, this.regionalData.unlocked_status);
    }

    private void unhighlightProvince()
    {
        this.SetColor();
        ProvinceManager.instance.HideProvinceDetails();
    }
}

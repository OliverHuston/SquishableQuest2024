using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class AlternativeButtonAnimations : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private float scaleIncreaseOnMoused = 1.10f;
    //[SerializeField] private float scaleIncreaseOnClicked = 1.20f;

    [SerializeField] private Color regularColor = Color.white;
    [SerializeField] private Color selectedColor = Color.green;

    [Space]
    [SerializeField] private string buttonGroupName = "";


    private Vector3 baseScale;
    private Image image;
    [HideInInspector] public bool altButtonSelected = false;

    private List<AlternativeButtonAnimations> buttonGroup;

    private void Awake()
    {
        baseScale = this.transform.localScale;
        image = this.transform.GetComponent<Image>();

        if (!buttonGroupName.Equals(""))
        {
            buttonGroup = new List<AlternativeButtonAnimations>();
            foreach (AlternativeButtonAnimations alternativeButton in FindObjectsOfType<AlternativeButtonAnimations>())
            {
                if (alternativeButton.buttonGroupName.Equals(this.buttonGroupName) && alternativeButton != this)
                {
                    buttonGroup.Add(alternativeButton);
                }
            }
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
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        altButtonSelected = !altButtonSelected;
        UpdateAppearance();
        if (altButtonSelected) DeselectOthersInButtonGroup();
    }

    private void DeselectOthersInButtonGroup()
    {
        foreach(AlternativeButtonAnimations alternativeButton in buttonGroup)
        {
            alternativeButton.altButtonSelected = false;
            alternativeButton.UpdateAppearance();
        }
    }

    public void UpdateAppearance()
    {
        if (altButtonSelected) { image.color = selectedColor; }
        else { image.color = regularColor; }
    }
}

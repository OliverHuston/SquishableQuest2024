using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChooseCharacterPortrait : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private int characterIndex;

    private Vector3 baseScale;
    private float scaleIncreaseOnMoused = 1.10f;

    void Awake()
    {
        baseScale = this.transform.localScale;
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        ChoosePartyManager.instance.DisplayCharacterDetails(characterIndex);
        this.transform.localScale = baseScale * scaleIncreaseOnMoused;
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        ChoosePartyManager.instance.HideCharacterDetails(characterIndex);
        this.transform.localScale = baseScale;

    }
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        ChoosePartyManager.instance.ProcessPortraitClick(characterIndex);
    }
}

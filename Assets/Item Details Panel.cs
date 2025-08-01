using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemDetailsPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text nameDisplay;
    [SerializeField] private TMP_Text rarityDisplay;
    [SerializeField] private TMP_Text descriptionDisplay;

    public void DisplayItem (Item item)
    {
        nameDisplay.text = item.itemName.ToUpper();
        rarityDisplay.text = item.RarityAsText().ToUpper();
        descriptionDisplay.text = item.displayDescription;
    }

}

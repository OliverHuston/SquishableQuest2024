using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsDisplayPanel : MonoBehaviour
{
    public Character source_character;
    private DisplayText[] displayTexts;
    private DisplayBar[] displayBars;

    void Awake()
    {
        displayTexts = GetComponentsInChildren<DisplayText>();
        displayBars = GetComponentsInChildren<DisplayBar>();
    }

    public void UpdateStats(Character newDisplayCharacter)
    {
        if (newDisplayCharacter == null) { 
            this.gameObject.SetActive(false);
            return; 
        }

        source_character = newDisplayCharacter;
        foreach(DisplayText dt in displayTexts)
        {
            dt.UpdateText(source_character);
        }
        foreach (DisplayBar db in displayBars)
        {
            db.UpdateBar(source_character);
        }
    }
}

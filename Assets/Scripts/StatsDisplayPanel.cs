using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsDisplayPanel : MonoBehaviour
{
    public Character source_character;
    private DisplayText[] displayTexts;

    void Awake()
    {
        displayTexts = GetComponentsInChildren<DisplayText>();
    }

    public void UpdateStats(Character newDisplayCharacter)
    {
        source_character = newDisplayCharacter;
        foreach(DisplayText dt in displayTexts)
        {
            dt.UpdateText(source_character);
        }
    }
}

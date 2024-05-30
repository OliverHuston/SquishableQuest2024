using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayBar : MonoBehaviour
{
    public Character source_character;

    public float barLength = 400;
    public string source_current;
    public string source_total;
    public bool preventNegative = true;

    // References variables
    private RectTransform rectTransform;

    // Setup
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update bar length based on provided new source_character.
    public void UpdateBar(Character newSourceCharacter)
    {
        source_character = newSourceCharacter;
        int current  = (int)source_character.GetType().GetField(source_current).GetValue(source_character);
        int total = (int)source_character.GetType().GetField(source_total).GetValue(source_character);
        float width = barLength * current / total;
        
        if(preventNegative && width < 0f) { width = 0f; }
        rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
    }
}

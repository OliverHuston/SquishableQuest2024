using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UI;

public class DisplayBar : MonoBehaviour
{
    private RectTransform rectTransform;

    public Character source_character;

    public float barLength = 400;

    public string source_current;
    public string source_total;

    // Start is called before the first frame update
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void UpdateBar(Character newSourceCharacter)
    {
        source_character = newSourceCharacter;
        int current  = (int)source_character.GetType().GetField(source_current).GetValue(source_character);
        int total = (int)source_character.GetType().GetField(source_total).GetValue(source_character);
        float width = barLength * current / total;
        rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
    }
}

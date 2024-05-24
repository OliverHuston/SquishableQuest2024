using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayText : MonoBehaviour
{
    private TMP_Text tmp;

    public Character source_character;
    public string source_var_name;

    void Awake()
    {
        tmp = GetComponent<TMP_Text>();
        UpdateText(source_character);
    }

    public void UpdateText(Character newSourceCharacter)
    {
        source_character = newSourceCharacter;
        string newDisplayString = (string)source_character.GetType().GetField(source_var_name).GetValue(source_character);
        tmp.text = newDisplayString;
    }
}

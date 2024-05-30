using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayText : MonoBehaviour
{
    public Character source_character;
    public string source_var_name;

    // Reference variables
    private TMP_Text tmp_text;

    // Setup
    void Awake()
    {
        tmp_text = GetComponent<TMP_Text>();
    }

    // Find variable matching source_variable_name the new source_character. (strings or ints only)
    public void UpdateText(Character newSourceCharacter)
    {
        source_character = newSourceCharacter;
        try
        {
            string newDisplayString = (string)source_character.GetType().GetField(source_var_name).GetValue(source_character);
            tmp_text.text = newDisplayString;
        }
        catch
        {
            int newDisplayInt = (int)source_character.GetType().GetField(source_var_name).GetValue(source_character);
            tmp_text.text = newDisplayInt + "";
        }
    }
}

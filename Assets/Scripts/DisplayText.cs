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
    }

    public void UpdateText(Character newSourceCharacter)
    {
        source_character = newSourceCharacter;
        //Debug.Log(source_var_name);
        try
        {
            string newDisplayString = (string)source_character.GetType().GetField(source_var_name).GetValue(source_character);
            tmp.text = newDisplayString;
        }
        catch
        {
            int newDisplayInt = (int)source_character.GetType().GetField(source_var_name).GetValue(source_character);
            tmp.text = newDisplayInt + "";
        }
    }
}

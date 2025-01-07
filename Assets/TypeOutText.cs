using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UI;
using TMPro;

public class TypeOutText : MonoBehaviour
{
    public string text = "SQUISHABLE QUEST.";
    private TMP_Text tMPro;
    private int count;

    // Start is called before the first frame update
    void Start()
    {
        tMPro = this.GetComponent<TMP_Text>();
        tMPro.text = "";
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        

        if (count < text.Length)
        {
            tMPro.text += text[count];
            count++;
        }
    }
}

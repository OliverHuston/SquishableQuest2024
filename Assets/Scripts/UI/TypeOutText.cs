using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypeOutText : MonoBehaviour
{
    [TextArea] public string text = "SQUISHABLE QUEST.";

    public IEnumerator TypeOut(float timeBetweenLetters, float randomAmount)
    {
        TMP_Text tMPro = this.GetComponent<TMP_Text>();
        tMPro.text = "";
        float timeElapsed = 0;
        float nextTime = 0;
        int count = 0;

        while (count < text.Length)
        {
            timeElapsed += Time.deltaTime;

            if (timeElapsed > nextTime)
            {
                //  Determine time to letter after this one.
                nextTime += (timeBetweenLetters + Random.Range(0, randomAmount));

                //  Add next letter.
                if (text[count] != '`') tMPro.text += text[count];
                count++;
            }
            yield return null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorLERP : MonoBehaviour
{
    public Color[] colors;


/*    public int currentColorIndex = 0;
    public int targetColorIndex = 1;
    public float time = 5;*/


    public IEnumerator Transition(int startColorIndex, int targetColorIndex, float time)
    {
        Image image = this.GetComponent<Image>();
        TMP_Text text = this.GetComponent<TMP_Text>();

        float targetPoint = 0;
        while (targetPoint/time < 1)
        {
            targetPoint += Time.deltaTime;
            if (text != null) { text.color = Color.Lerp(colors[startColorIndex], colors[targetColorIndex], targetPoint / time); }
            else if (image != null) { image.color = Color.Lerp(colors[startColorIndex], colors[targetColorIndex], targetPoint / time); }
            yield return null;
        }
    }

}

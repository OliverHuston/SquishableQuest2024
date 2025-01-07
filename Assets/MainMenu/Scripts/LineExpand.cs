using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UI;

public class LineExpand : MonoBehaviour
{
    public float startWidth = 2;
    public float height = 2;
    public float endWidth = 100;

    public float startY = 0;
    public float endY = 50;

    public float time = 2;


    RectTransform rectTransform;

    // Start is called before the first frame update
    void Awake()
    {
        rectTransform = this.GetComponent<RectTransform>();

        rectTransform.sizeDelta = new Vector2(startWidth, height);
    }

    void Update()
    {
        if(rectTransform.sizeDelta.x < endWidth)
        {
            rectTransform.sizeDelta += new Vector2(Time.deltaTime * (endWidth-startWidth)/time, 0);
            rectTransform.position += new Vector3(0, Time.deltaTime * (endY-startY)/time , 0);
        }
    }
}

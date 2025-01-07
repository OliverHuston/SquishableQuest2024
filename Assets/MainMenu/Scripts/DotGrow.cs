using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotGrow : MonoBehaviour
{
    public float minScale = 5;
    public float maxScale = 30;
    public float time = 5;

    RectTransform rectTransform;

    void Start()
    {
        rectTransform = this.GetComponent<RectTransform>();

    }

    // Update is called once per frame
    void Update()
    {
        float a = minScale + maxScale*Mathf.Sin(Time.deltaTime/time);
        rectTransform.sizeDelta = new Vector2(a, a);
    }
}

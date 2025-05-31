using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZoomDependentFade : MonoBehaviour
{
    [SerializeField] private Color start_color = Color.clear;
    [SerializeField] private Color end_color = Color.white;

    [SerializeField] private float start_threshold = 550f;
    [SerializeField] private float end_threshold = 600f;


    private Image image;

    // Start is called before the first frame update
    void Awake()
    {
        image = this.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        float zoom_amount = Camera.main.orthographicSize;

        if(zoom_amount < start_threshold) { image.color = start_color; }
        else if(zoom_amount > end_threshold) { image.color = end_color; }
        else
        {
            float factor = 1 - (end_threshold - zoom_amount) / (end_threshold - start_threshold);
            image.color = Color.Lerp(start_color, end_color, factor);
        }
    }
}

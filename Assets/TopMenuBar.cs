using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopMenuBar : MonoBehaviour
{
    [SerializeField] private Transform[] ui_windows;
    [SerializeField] private GameObject[] topMenuButtons;

    int activeWindowIndex = -1;

    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RegisterIndexClick(int window_index)
    {
        for(int i = 0; i < ui_windows.Length; i++)
        {
            if(i == window_index) {
                ui_windows[i].gameObject.SetActive(!ui_windows[i].gameObject.activeSelf);
            }
            else
            {
                ui_windows[i].gameObject.SetActive(false);
            }
        }
    }

}

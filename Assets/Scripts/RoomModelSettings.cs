using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomModelSettings : MonoBehaviour
{
    public bool dynamicLighting = true;
    public bool dynamicAnimations = true;

    // Start is called before the first frame update
    void Awake()
    {
        UpdateSettings();
    }

    private void UpdateSettings()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.GetComponent<Light>() != null)
            {
                if (dynamicLighting) child.SetActive(true);
                else { child.SetActive(false); }
            }
            else if (child.GetComponent<Animator>() != null)
            {
                if (dynamicAnimations) child.SetActive(true);
                else { child.SetActive(false); }
            }
        }
    }
}

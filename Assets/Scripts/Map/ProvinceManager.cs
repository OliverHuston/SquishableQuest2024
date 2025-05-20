using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProvinceManager : MonoBehaviour
{
    [SerializeField] private GameObject provinceInfoDisplay;

    public static ProvinceManager instance { get; private set; }

    private TMP_Text provinceNameText;
    private TMP_Text provinceDescriptionText;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one ProvinceManager in the scene.");
        }
        instance = this;

        provinceNameText = provinceInfoDisplay.transform.GetChild(0).GetComponent<TMP_Text>();
        provinceDescriptionText = provinceInfoDisplay.transform.GetChild(1).GetComponent<TMP_Text>();
        provinceInfoDisplay.SetActive(false);
    }

    public void DisplayProvinceDetails(string name, string description, bool available, bool unlocked)
    {
        provinceNameText.text = "-"+name.ToUpper()+"-";
        provinceDescriptionText.text = description;
        if (!available) { SetAvailabilityDisplay(0); }
        else if (!unlocked) { SetAvailabilityDisplay(1); }
        else { SetAvailabilityDisplay(2); }

        provinceInfoDisplay.SetActive(true);
    }

    public void HideProvinceDetails()
    {
        provinceInfoDisplay.SetActive(false);
    }


    //  0 = unavailable, 1 = locked, 2 = unlocked
    private void SetAvailabilityDisplay(int status)
    {
        if(status == 0)
        {
            provinceInfoDisplay.transform.GetChild(2).gameObject.SetActive(true); 
            provinceInfoDisplay.transform.GetChild(3).gameObject.SetActive(false);
            provinceInfoDisplay.transform.GetChild(4).gameObject.SetActive(false);

        }
        else if (status == 1)
        {
            provinceInfoDisplay.transform.GetChild(2).gameObject.SetActive(false);
            provinceInfoDisplay.transform.GetChild(3).gameObject.SetActive(true);
            provinceInfoDisplay.transform.GetChild(4).gameObject.SetActive(false);

        }
        if (status == 2)
        {
            provinceInfoDisplay.transform.GetChild(2).gameObject.SetActive(false);
            provinceInfoDisplay.transform.GetChild(3).gameObject.SetActive(false);
            provinceInfoDisplay.transform.GetChild(4).gameObject.SetActive(true);
        }
    }
}

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

    public void DisplayProvinceDetails(string name, string description, RegionUnlocked unlocked_status)
    {
        provinceNameText.text = "-"+name.ToUpper()+"-";
        provinceDescriptionText.text = description;
        SetAvailabilityDisplay(unlocked_status);
        provinceInfoDisplay.SetActive(true);
    }

    public void HideProvinceDetails()
    {
        provinceInfoDisplay.SetActive(false);
    }


    private void SetAvailabilityDisplay(RegionUnlocked status)
    {
        if(status == RegionUnlocked.NOT_YET_AVAILABLE)
        {
            provinceInfoDisplay.transform.GetChild(2).gameObject.SetActive(true); 
            provinceInfoDisplay.transform.GetChild(3).gameObject.SetActive(false);
            provinceInfoDisplay.transform.GetChild(4).gameObject.SetActive(false);
            provinceInfoDisplay.transform.GetChild(5).gameObject.SetActive(false);

        }
        else if (status == RegionUnlocked.COMING_SOON)
        {
            provinceInfoDisplay.transform.GetChild(2).gameObject.SetActive(false);
            provinceInfoDisplay.transform.GetChild(3).gameObject.SetActive(true);
            provinceInfoDisplay.transform.GetChild(4).gameObject.SetActive(false);
            provinceInfoDisplay.transform.GetChild(5).gameObject.SetActive(false);

        }
        else if (status == RegionUnlocked.LOCKED)
        {
            provinceInfoDisplay.transform.GetChild(2).gameObject.SetActive(false);
            provinceInfoDisplay.transform.GetChild(3).gameObject.SetActive(false);
            provinceInfoDisplay.transform.GetChild(4).gameObject.SetActive(true);
            provinceInfoDisplay.transform.GetChild(5).gameObject.SetActive(false);
        }
        else if (status == RegionUnlocked.UNLOCKED)
        {
            provinceInfoDisplay.transform.GetChild(2).gameObject.SetActive(false);
            provinceInfoDisplay.transform.GetChild(3).gameObject.SetActive(false);
            provinceInfoDisplay.transform.GetChild(4).gameObject.SetActive(false);
            provinceInfoDisplay.transform.GetChild(5).gameObject.SetActive(true);
        }
    }
}

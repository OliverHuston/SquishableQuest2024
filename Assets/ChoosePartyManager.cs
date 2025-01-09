using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoosePartyManager : MonoBehaviour
{
    [SerializeField] private Color unselectedColor;
    [SerializeField] private Color selectedColor;

    [SerializeField] private string[] characterNames;

    [Space]
    [SerializeField] private GameObject characterDetailsPanel;
    [SerializeField] private GameObject characterPortraitsPanel;

    [Space]
    [SerializeField] private GameObject chosenPartyDisplay;

    [Space]
    [SerializeField] private GameObject startAdventureButton;


    private GameObject[] characterDisplaysArray;
    private List<int> chosenCharacters = new List<int>();

    public static ChoosePartyManager instance { get; private set; }
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one ChoosePartyManager in the scene.");
        }
        instance = this;


        characterDisplaysArray = new GameObject[characterDetailsPanel.transform.childCount];
        for(int i = 0; i < characterDisplaysArray.Length; i++)
        {
            characterDisplaysArray[i] = characterDetailsPanel.transform.GetChild(i).gameObject;
        }

        chosenPartyDisplay.SetActive(true);
        startAdventureButton.SetActive(false);
        foreach(GameObject display in characterDisplaysArray)
        {
            if(display != null) display.SetActive(false);
        }


        ConfigureChosenPartyDisplay();

    }


    public void DisplayCharacterDetails(int index)
    {
        characterDisplaysArray[index].SetActive(true);
        chosenPartyDisplay.SetActive(false);
        this.UpdateDisplays();
    }
    public void HideCharacterDetails(int index)
    {
        characterDisplaysArray[index].SetActive(false);
        chosenPartyDisplay.SetActive(true);
        this.UpdateDisplays();
    }

    public void ProcessPortraitClick(int index)
    {
        if (chosenCharacters.Count < 4 && !chosenCharacters.Contains(index))
        {
            chosenCharacters.Add(index);
            this.UpdateDisplays();
        }
        else if (chosenCharacters.Contains(index))
        {
            RemoveFromParty(chosenCharacters.IndexOf(index));
        }
    }

    public void RemoveFromParty(int index)
    {
        Debug.Log("Removing index " + index + " = " + characterNames[chosenCharacters[index]]);
        chosenCharacters.RemoveAt(index);
        this.UpdateDisplays();
    }



    private void UpdateDisplays()
    {
        if(chosenCharacters.Count == 4 && chosenPartyDisplay.activeSelf) { startAdventureButton.SetActive(true); }
        else { startAdventureButton.SetActive(false); }

        chosenCharacters.Sort();
        this.ConfigureCharacterPortraits();
        this.ConfigureChosenPartyDisplay();
    }

    private void ConfigureCharacterPortraits()
    {
        for(int i = 0; i < characterPortraitsPanel.transform.childCount; i++)
        {
            Image portraitBackdrop = characterPortraitsPanel.transform.GetChild(i).GetChild(0).GetComponent<Image>();
            if (chosenCharacters.Contains(i))
            {
                portraitBackdrop.color = selectedColor;
            }
            else
            {
                portraitBackdrop.color = unselectedColor;
            }
        }
    }

    private void ConfigureChosenPartyDisplay()
    {
        chosenPartyDisplay.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = "PARTY (" + chosenCharacters.Count + "/4)";
        for(int i = 0; i < 4; i++)
        {
            Transform namePanelTransform = chosenPartyDisplay.transform.GetChild(i + 1);
            if (i < chosenCharacters.Count) { 
                namePanelTransform.gameObject.SetActive(true);
                namePanelTransform.GetChild(1).GetComponent<TMP_Text>().text = characterNames[chosenCharacters[i]].ToUpper();
            }
            else { namePanelTransform.gameObject.SetActive(false); }
        }
    }


}

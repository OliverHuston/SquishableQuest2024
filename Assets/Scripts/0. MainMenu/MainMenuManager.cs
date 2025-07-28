using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public ColorLERP background;
    public GameObject titleText;
    public GameObject anyButtonToPlayMessage;
    public GameObject partySelector;

    public GameObject[] newGameButtons;
    public GameObject[] loadGameButtons;

    [Space] [SerializeField] private bool skipIntroSequence;

    private bool  buttonPressed = false;

    void Start()
    {
        SetupStartGameButtons();
        if(!skipIntroSequence) StartCoroutine(GameLaunchSequence());
        else
        {
            partySelector.SetActive(true);
            Cursor.visible = true;
        }
    }

    // Manipulation of graphics elements for game start up sequence.
    private IEnumerator GameLaunchSequence()
    {
        titleText.SetActive(false);
        anyButtonToPlayMessage.SetActive(false);
        partySelector.SetActive(false);
        Cursor.visible = false;

        // Fade in from black to display title.
        yield return background.Transition(0, 1, 3f);
        //yield return new WaitForSeconds(1f);

        titleText.SetActive(true);
        yield return titleText.GetComponent<ColorLERP>().Transition(1, 0, 3f);
        //yield return titleText.GetComponent<TypeOutText>().TypeOut(.1f, .1f);
        yield return new WaitForSeconds(.01f);

        // Player must press any key to continue.
        anyButtonToPlayMessage.SetActive(true);
        buttonPressed = false;
        yield return anyButtonToPlayMessage.GetComponent<ColorLERP>().Transition(0, 2, 1.5f);
        while (!buttonPressed) {
            yield return anyButtonToPlayMessage.GetComponent<ColorLERP>().Transition(2, 1, 1f);
            yield return anyButtonToPlayMessage.GetComponent<ColorLERP>().Transition(1, 2, 1f);
        }

        // Fade to brown background.
        StartCoroutine(anyButtonToPlayMessage.GetComponent<ColorLERP>().Transition(2, 0, .5f));
        StartCoroutine(titleText.GetComponent<ColorLERP>().Transition(0, 1, 2f));
        yield return background.Transition(1, 2, 2.5f);

        // Party selection screen drops down from top (with slight bounce).
        partySelector.SetActive(true);
        yield return partySelector.GetComponent<MoveUI>().Move(0, 1, .5f);
        yield return partySelector.GetComponent<MoveUI>().Move(1, 2, .1f);
        yield return partySelector.GetComponent<MoveUI>().Move(2, 1, .1f);
        yield return new WaitForSeconds(.25f);
        Cursor.visible = true;
    }

    // Check for input.
    void Update()
    {
        if(Input.anyKey) { buttonPressed = true; }
    }


    private void SetupStartGameButtons()
    {
        for(int i = 0; i < newGameButtons.Length; i++)
        {
            bool slotActive = DataPersistenceManager.instance.hasData(i + 1);
            newGameButtons[i].SetActive(!slotActive);
            loadGameButtons[i].SetActive(slotActive);

            if(slotActive)
            {
                loadGameButtons[i].transform.GetChild(2).GetComponent<TMP_Text>().text = DataPersistenceManager.instance.getPartyName(i + 1);
            }
        }
    }

    // NAVIGATION TO OTHER LEVELS
    public void LoadGame(int saveSlot) {
        DataPersistenceManager.instance.LoadGame(saveSlot);
        Debug.Log("Game loading from Save Slot " + saveSlot + ".");
        ASyncLoader.instance.LoadLevelBtn("ChooseParty");
    }

    public void CreateNewGame(int saveSlot)
    {
        DataPersistenceManager.instance.NewGame(saveSlot);
        Debug.Log("Game created in Save Slot " + saveSlot + ".");
        ASyncLoader.instance.LoadLevelBtn("ChooseParty");
    }
}
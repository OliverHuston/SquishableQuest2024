using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public ColorLERP background;
    public GameObject titleText;
    public GameObject anyButtonToPlayMessage;
    public GameObject partySelector;

    [SerializeField] private ASyncLoader aSyncLoader;

    private bool  buttonPressed = false;

    void Start()
    {
        StartCoroutine(GameLaunchSequence());
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
        titleText.SetActive(true);
        yield return titleText.GetComponent<TypeOutText>().TypeOut(.1f, .1f);
        yield return new WaitForSeconds(1f);

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

    // NAVIGATION TO OTHER LEVELS
    public void LoadGame() { }

    public void CreateNewGame(int saveNumber)
    {
        Debug.Log("Game created in save slot #" + saveNumber + ".");
        aSyncLoader.LoadLevelBtn("ChooseParty");
    }
}

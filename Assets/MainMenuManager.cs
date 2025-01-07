using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public ColorLERP background;
    public GameObject titleText;
    public GameObject anyButtonToPlayMessage;

    private bool  buttonPressed = false;

    void Start()
    {
        StartCoroutine(StartSequence());
    }

    private IEnumerator StartSequence()
    {
        titleText.SetActive(false);
        anyButtonToPlayMessage.SetActive(false);

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
            yield return anyButtonToPlayMessage.GetComponent<ColorLERP>().Transition(2, 1, .75f);
            yield return anyButtonToPlayMessage.GetComponent<ColorLERP>().Transition(1, 2, .75f);
        }

        // Fade to brown background.
        StartCoroutine(anyButtonToPlayMessage.GetComponent<ColorLERP>().Transition(2, 0, 2f));
        StartCoroutine(titleText.GetComponent<ColorLERP>().Transition(0, 1, 2f));
        yield return background.Transition(1, 2, 5f);

    }

    void Update()
    {
        if(Input.anyKey) { buttonPressed = true; }
    }
}

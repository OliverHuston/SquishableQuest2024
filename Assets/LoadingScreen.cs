using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UnityEngine.Random;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text loadingScreenTipText;
    [Space]

    [SerializeField] private string[] loadingScreenTips;

    public void DisplayNewTip()
    {
        int index = Random.Range(0, loadingScreenTips.Length);
        loadingScreenTipText.text = loadingScreenTips[index];
    }
}

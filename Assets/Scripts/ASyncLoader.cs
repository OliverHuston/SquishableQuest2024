using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ASyncLoader : MonoBehaviour
{
    [Header("Menu Screens")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject sceneUI;

    private Slider loadingSlider;

    public static ASyncLoader instance { get; private set; }
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one ASyncLoader in the scene.");
        }
        instance = this;
        loadingSlider = loadingScreen.transform.GetChild(0).GetChild(0).GetComponent<Slider>();
    }

        public void LoadLevelBtn(string levelToLoad)
    {
        sceneUI.SetActive(false);
        loadingScreen.SetActive(true);
        loadingScreen.GetComponent<LoadingScreen>().DisplayNewTip();

        StartCoroutine(LoadLevelAsync(levelToLoad));
    }


    IEnumerator LoadLevelAsync(string levelToLoad) {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelToLoad);

        while(!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            loadingSlider.value = progressValue;
            yield return null;
        }
    }
}

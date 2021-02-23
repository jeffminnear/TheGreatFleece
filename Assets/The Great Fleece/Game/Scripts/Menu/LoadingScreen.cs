using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public Image progressBar;

    void Start()
    {
        StartCoroutine(LoadMainScene());
    }

    IEnumerator LoadMainScene()
    {
        AsyncOperation loadNextScene = SceneManager.LoadSceneAsync("Main");

        while (loadNextScene.isDone == false)
        {
            progressBar.fillAmount = loadNextScene.progress;

            yield return new WaitForEndOfFrame();
        }
    }
}

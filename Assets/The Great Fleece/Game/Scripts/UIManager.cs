using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("UIManager is NULL!");
            }

            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void Restart(bool showIntroCutscene)
    {
        SessionManager.PlayStartLevelCutscene = showIntroCutscene;
        Time.timeScale = 1f;
        SceneManager.LoadScene("LoadingScreen");
    }

    public void Quit()
    {
        Debug.Log("Quitting application...");
        Application.Quit();
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Quit()
    {
        Debug.Log("Quitting application...");
        Application.Quit();
    }

    public void StartGame()
    {
        SessionManager.PlayStartLevelCutscene = true;
        SceneManager.LoadScene("Main");
    }
}

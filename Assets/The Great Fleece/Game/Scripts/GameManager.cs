using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("GameManager is NULL!");
            }

            return _instance;
        }
    }

    public GameObject gameOverCutscene;

    public bool gameIsActive { get; private set; } = true;
    public bool PlayerHasCard { get; set; } = false;
    private string currentCutsceneId = null;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    public void GameOver()
    {
        if (gameOverCutscene != null)
        {
            gameOverCutscene.SetActive(true);
            gameIsActive = false;
        }
    }

    public void StartCutscene(string name)
    {
        currentCutsceneId = name;
        gameIsActive = false;
    }

    public void EndCutscene()
    {
        if (currentCutsceneId == null)
        {
            return;
        }
        
        GameObject.Find(currentCutsceneId).SetActive(false);
        currentCutsceneId = null;
        gameIsActive = true;
    }
}

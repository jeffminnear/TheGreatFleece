using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

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

    public GameObject startLevelCutscene;
    public GameObject gameOverCutscene;
    public GameObject winLevelCutscene;
    public GameObject player;

    public bool gameIsActive { get; private set; } = true;
    public bool PlayerHasCard { get; set; } = false;
    private string currentCutsceneId = null;
    private GameObject pauseCanvas;

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

    void Start()
    {
        pauseCanvas = GameObject.Find("PauseCanvas");

        Helpers.Validation.VerifyReferences(gameObject, pauseCanvas);

        pauseCanvas.SetActive(false);

        if (SessionManager.PlayStartLevelCutscene)
        {
            startLevelCutscene.SetActive(true);
        }
    }

    void Update()
    {
        if (currentCutsceneId != null)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                PlayableDirector director = GameObject.Find(currentCutsceneId).GetComponent<PlayableDirector>();
                double fadeOutPoint = director.duration - 0.5;
                director.time = fadeOutPoint;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (Time.timeScale == 0f)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        gameIsActive = false;
        pauseCanvas.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        gameIsActive = true;
        pauseCanvas.SetActive(false);
    }

    public void GameOver()
    {
        if (gameOverCutscene != null)
        {
            gameOverCutscene.SetActive(true);
        }
    }

    public void WinLevel()
    {
        if (winLevelCutscene != null)
        {
            winLevelCutscene.SetActive(true);
        }
    }

    public void StartCutscene(string name)
    {
        currentCutsceneId = name;
        gameIsActive = false;
        AudioManager.Instance.StopAll();
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

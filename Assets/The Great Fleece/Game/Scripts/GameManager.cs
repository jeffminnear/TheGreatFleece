using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] entities;
    public GameObject gameOverCutscene;

    public bool gameIsActive { get; private set; } = true;
    private string currentCutsceneId = null;

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

    private IEnumerator DestroyEntities()
    {
        yield return new WaitForSeconds(0.05f);

        if (entities.Length > 0)
        {
            foreach (GameObject entity in entities)
            {
                Destroy(entity);
            }
        }
    }
}

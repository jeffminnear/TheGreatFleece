using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] entities;
    public GameObject gameOverCutscene;

    public void GameOver()
    {
        if (gameOverCutscene != null)
        {
            gameOverCutscene.SetActive(true);
        }
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

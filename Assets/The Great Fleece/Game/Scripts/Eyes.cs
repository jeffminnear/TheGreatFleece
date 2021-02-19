using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Helpers.Validation;

public class Eyes : MonoBehaviour
{
    private GameManager _gameManager;

    void Awake()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (_gameManager == null)
        {
            Debug.LogError("GameManager is missing!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _gameManager.GameOver();
        }
    }
}

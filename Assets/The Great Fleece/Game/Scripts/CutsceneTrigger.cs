using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    public GameObject cutscene;

    private bool hasTriggeredCutscene = false;

    void Awake()
    {
        Helpers.Validation.VerifyReferences(gameObject, cutscene);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!hasTriggeredCutscene)
            {
                hasTriggeredCutscene = true;
                cutscene.SetActive(true);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    public GameObject cutscene;

    [SerializeField]
    private bool grantsPlayerCard = false;
    [SerializeField]
    private bool requiresPlayerHasCard = false;
    private bool hasTriggeredCutscene = false;

    void Awake()
    {
        Helpers.Validation.VerifyReferences(gameObject, cutscene);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (requiresPlayerHasCard && !GameManager.Instance.PlayerHasCard)
            {
                return;
            }

            if (!hasTriggeredCutscene)
            {
                if (grantsPlayerCard)
                {
                    GameManager.Instance.PlayerHasCard = true;
                }
                
                hasTriggeredCutscene = true;
                cutscene.SetActive(true);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceOverTrigger : MonoBehaviour
{
    public AudioClip voiceOverClip;

    private Speaker speaker;
    private bool hasPlayedClip = false;

    void Start()
    {
        speaker = GameObject.Find("Speaker").GetComponent<Speaker>();

        Helpers.Validation.VerifyComponents(gameObject, speaker);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!hasPlayedClip)
            {
                hasPlayedClip = true;
                speaker.PlayClip(voiceOverClip, 0.5f);
            }
        }
    }
}

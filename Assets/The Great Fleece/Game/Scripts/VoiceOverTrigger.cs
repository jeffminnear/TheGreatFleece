using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class VoiceOverTrigger : MonoBehaviour
{
    private AudioSource audioSource;
    private bool hasPlayedClip = false;

    void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();

        Helpers.Validation.VerifyComponents(gameObject, audioSource);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!hasPlayedClip)
            {
                hasPlayedClip = true;
                audioSource.Play();
            }
        }
    }
}

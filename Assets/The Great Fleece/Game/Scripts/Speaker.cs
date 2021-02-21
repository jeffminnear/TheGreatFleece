using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Speaker : MonoBehaviour
{
    private AudioSource s_AudioSource;

    void Awake()
    {
        s_AudioSource = gameObject.GetComponent<AudioSource>();

        Helpers.Validation.VerifyComponents(gameObject, s_AudioSource);
    }

    public void PlayClip(AudioClip clip, float volume = 1f)
    {
        s_AudioSource.PlayOneShot(clip, volume);
    }
}

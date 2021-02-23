using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceOverTrigger : MonoBehaviour
{
    public AudioClip voiceOverClip;

    private bool hasPlayedClip = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!hasPlayedClip)
            {
                hasPlayedClip = true;
                AudioManager.Instance.PlayVO(voiceOverClip);
            }
        }
    }
}

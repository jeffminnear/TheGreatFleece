using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("AudioManager is NULL!");
            }

            return _instance;
        }
    }
    private AudioSource VO;
    private AudioSource SFX;
    private AudioSource Music;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }

        VO = transform.Find("VO").GetComponent<AudioSource>();
        VO.volume = 0.35f;
        SFX = transform.Find("SFX").GetComponent<AudioSource>();
        SFX.volume = 0.3f;
        Music = transform.Find("Music").GetComponent<AudioSource>();
        Music.volume = 0.4f;

        Helpers.Validation.VerifyComponents(gameObject, VO, SFX, Music);

        Music.Play();
    }

    void Update()
    {
        if (GameManager.Instance.gameIsActive)
        {
            if (!Music.isPlaying)
            {
                Music.Play();
            }
        }
        else
        {
            if (Music.isPlaying)
            {
                Music.Stop();
            }
        }
    }

    public void PlayVO(AudioClip clip, float volume = 1f)
    {
        VO.PlayOneShot(clip, volume);
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        SFX.PlayOneShot(clip, volume);
    }
}

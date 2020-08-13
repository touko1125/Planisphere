﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    [SerializeField]
    private GameObject[] AudioSourceObjects;

    public AudioSource[] AudioSources;

    public AudioClip[] AudioClips;

    public float volumeBGM = 0.2f;

    public float volumeSE = 0.8f;

    private void Awake()
    {
        //要素の確保
        AudioSources = new AudioSource[AudioSourceObjects.Length];

        for (int i = 0; i < AudioSourceObjects.Length; i++)
        {
            AudioSources[i] = AudioSourceObjects[i].GetComponent<AudioSource>();
        }

        AudioSources[0].volume = volumeBGM;
        AudioSources[1].volume = volumeSE;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeVolume(float volume,int audioSourceNum)
    {
        AudioSources[audioSourceNum].volume = volume;
        switch (audioSourceNum)
        {
            case 0:
                volumeBGM = volume;
                break;
            case 1:
                volumeSE = volume;
                break;
        }
;   }

    public void PlayAudio(AudioClip audioClip,AudioSource audioSource,float volume,bool is_loop)
    {
        audioSource.volume = volume;
        audioSource.loop = is_loop;
        audioSource.PlayOneShot(audioClip);
    }
}

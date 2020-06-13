using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    public GameObject[] AudioSourceObjects;
    public AudioSource[] AudioSources;

    public AudioClip[] AudioClips;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        //要素の確保
        AudioSources = new AudioSource[AudioSourceObjects.Length];

        for(int i = 0; i < AudioSourceObjects.Length; i++)
        {
            AudioSources[i] = AudioSourceObjects[i].GetComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAudio(AudioClip audioClip,AudioSource audioSource,float volume,bool is_loop)
    {
        audioSource.volume = volume;
        audioSource.loop = is_loop;
        audioSource.PlayOneShot(audioClip);
    }
}

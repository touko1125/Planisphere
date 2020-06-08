using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public GameObject[] AudioSourceObjects;
    public AudioSource[] AudioSources;

    public AudioClip[] AudioClips;

    public static AudioManager audiomanager;

    private void Awake()
    {
        if (audiomanager == null)
        {
            audiomanager = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
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

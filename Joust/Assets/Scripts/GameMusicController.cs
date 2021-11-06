using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMusicController : MonoBehaviour
{
    public static GameMusicController instance;

    public AudioClip mainSong;
    AudioSource mainSongSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mainSongSource = AddAudio(true, false, 0.5f);
        mainSongSource.clip = mainSong;
        mainSongSource.Play();
    }

    public AudioSource AddAudio(bool loop, bool playOnAwake, float vol)
    {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        newAudio.loop = loop;
        newAudio.playOnAwake = playOnAwake;
        newAudio.volume = vol;
        return newAudio;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StopMusic() {
        mainSongSource.Stop();
    }
}

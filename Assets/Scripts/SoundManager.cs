using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null; // Creates an instance of the Sound Manager
    public AudioSource efxSource; //Drag a reference to the audio source which will play the sound effects.
    public AudioSource musicSource; //Drag a reference to the audio source which will play the music.
    public AudioSource switchedMusicSource;
    private bool switchedMusicPlaying;

    /**
     * Creates an instance of the Sound Manager
     */
    void Awake ()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        switchedMusicPlaying = false;
    }

    /**
     * Plays a single sound clip
     */
    public void PlaySingle(AudioClip clip)
    {
        //Set the clip of our efxSource audio source to the clip passed in as a parameter.
        efxSource.clip = clip;

        //Play the clip.
        efxSource.Play();
    }

    /**
     * Sets the background music of the game
     */
    public void SetBackgroundMusic(AudioClip music)
    {
        if(musicSource.isPlaying)
        {
            switchedMusicPlaying = false;

            // Set the background music
            switchedMusicSource.clip = music;
            switchedMusicSource.volume = 0;
            switchedMusicSource.Play();

            InvokeRepeating("SwitchMusic", 0f, 0.01f);
        }
        else
        {
            switchedMusicPlaying = true;

            // Set the background music
            musicSource.clip = music;
            musicSource.volume = 0;
            musicSource.Play();

            InvokeRepeating("SwitchMusic", 0f, 0.01f);
        }
    }

    void SwitchMusic()
    {
        if (!switchedMusicPlaying)
        {
            if (switchedMusicSource.volume < 1)
            {
                switchedMusicSource.volume = switchedMusicSource.volume + 0.01f;
                musicSource.volume = musicSource.volume - 0.01f;
            }
            else
            {
                CancelInvoke();
                musicSource.Stop();
            }
        }
        else
        {
            if (musicSource.volume < 1)
            {
                musicSource.volume = musicSource.volume + 0.01f;
                switchedMusicSource.volume = switchedMusicSource.volume - 0.01f;
            }
            else
            {
                CancelInvoke();
                switchedMusicSource.Stop();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null; // Creates an instance of the Sound Manager
    public AudioSource efxSource; //Drag a reference to the audio source which will play the sound effects.
    public AudioSource secondaryEfxSource; // If the primary efxSource is busy
    public AudioSource musicSource; //Drag a reference to the audio source which will play the music.
    public AudioSource secondaryMusicSource; // The music to be switched to for background music (used for cross-fade)
    private bool secondaryMusicPlaying;

    public AudioClip cannonSound;
    public AudioClip explosionPlayerSound;
    public AudioClip explosionEnemySound;
    public AudioClip owenWilsonBoom;
    public AudioClip owenWilsonStart;

    private Hashtable soundEffects;

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

        secondaryMusicPlaying = false;

        soundEffects = new Hashtable();
        soundEffects.Add("cannonFire", cannonSound);
        soundEffects.Add("explosionPlayer", explosionPlayerSound);
        soundEffects.Add("explosionEnemy", explosionEnemySound);
        soundEffects.Add("startLevel", null); // TODO replace with something
    }

    void InitSounds()
    {
        soundEffects["cannonFire"] = cannonSound;
        soundEffects["explosionPlayer"] = explosionPlayerSound;
        soundEffects["explosionEnemy"] = explosionEnemySound;
        soundEffects["startLevel"] = null; // TODO replace with something
    }

    /**
     * Toggles the sound to Owen Wilson
     */
    public void Wow()
    {
        if ((AudioClip)soundEffects["explosionEnemy"] != owenWilsonBoom)
        {
            soundEffects["explosionEnemy"] = owenWilsonBoom;
            soundEffects["startLevel"] = owenWilsonStart;
            PlaySingle("startLevel");
        }
        else
            InitSounds();
    }

    /**
     * Plays a single sound clip
     */
    public void PlaySingle(string clipName)
    {
        // Get the sound clip
        AudioClip clip = (AudioClip)soundEffects[clipName]; // If it doesn't exist, we don't really care

        if(efxSource.isPlaying) // If primary efxSource is busy
        {
            //Set the clip of our efxSource audio source to the clip passed in as a parameter.
            efxSource.clip = clip;

            //Play the clip.
            efxSource.Play();
        }
        else
        {
            //Set the clip of our efxSource audio source to the clip passed in as a parameter.
            secondaryEfxSource.clip = clip;

            //Play the clip.
            secondaryEfxSource.Play();
        }
    }

    /**
     * Sets the background music of the game
     */
    public void SetBackgroundMusic(AudioClip music)
    {
        if(musicSource.isPlaying) // If the musicSource object is the one currently making sound
        {
            secondaryMusicPlaying = false;

            // Set the background music
            secondaryMusicSource.clip = music;
            secondaryMusicSource.volume = 0;
            secondaryMusicSource.Play();

            InvokeRepeating("SwitchMusic", 0f, 0.01f); // Begin the cross-fade
        }
        else
        {
            secondaryMusicPlaying = true;

            // Set the background music
            musicSource.clip = music;
            musicSource.volume = 0;
            musicSource.Play();

            InvokeRepeating("SwitchMusic", 0f, 0.01f);
        }
    }

    /**
     * Cross-fades between the two music tracks
     */
    void SwitchMusic()
    {
        if (!secondaryMusicPlaying) // If the musicSource object is the one playing currently
        {
            if (secondaryMusicSource.volume < 1)
            {
                secondaryMusicSource.volume = secondaryMusicSource.volume + 0.01f; // Increase switched music volume
                musicSource.volume = musicSource.volume - 0.01f; // Decrease music source volume
            }
            else // If the switched music source is at 1 (and musicSource is muted)
            {
                CancelInvoke("SwitchMusic"); // Stop repeating
                musicSource.Stop(); // Stop the music source
            }
        }
        else
        {
            if (musicSource.volume < 1)
            {
                musicSource.volume = musicSource.volume + 0.01f;
                secondaryMusicSource.volume = secondaryMusicSource.volume - 0.01f;
            }
            else
            {
                CancelInvoke("SwitchMusic");
                secondaryMusicSource.Stop();
            }
        }
    }
}

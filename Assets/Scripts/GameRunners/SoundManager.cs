using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null; // Creates an instance of the Sound Manager
    public AudioSource efxSource; //Drag a reference to the audio source which will play the sound effects.
    public AudioSource secondaryEfxSource; // If the primary efxSource is busy
    public AudioSource tertiaryEfxSource; // If the secondary efxSource is busy
    public AudioSource quaternaryEfxSource; // If the tertiary efxSource is busy
    public AudioSource musicSource; //Drag a reference to the audio source which will play the music.
    public AudioSource secondaryMusicSource; // The music to be switched to for background music (used for cross-fade)
    private bool secondaryMusicPlaying;
    private bool waitForMusic;

    public AudioClip cannonPlayerSound;
    public AudioClip cannonEnemySound;
    public AudioClip playerDeathSound;
    public AudioClip explosionEnemySound;
    public AudioClip levelStartSound;
    public AudioClip driveBySound;

    public AudioClip owenWilsonBoom;
    public AudioClip owenWilsonStart;

    public AudioClip philStart;
    public AudioClip majorDamage;
    public AudioClip philEndGame;
    public AudioClip philStartGame;

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
        soundEffects.Add("cannonPlayerFire", cannonPlayerSound);
        soundEffects.Add("cannonEnemyFire", cannonEnemySound);
        soundEffects.Add("playerDeath", playerDeathSound);
        soundEffects.Add("explosionEnemy", explosionEnemySound);
        soundEffects.Add("gameStart", levelStartSound);
        soundEffects.Add("driveBy", driveBySound);
    }

    /**
     * Resets the sound effects
     */
    void InitSounds()
    {
        soundEffects["cannonPlayerFire"] = cannonPlayerSound;
        soundEffects["cannonEnemyFire"] = cannonEnemySound;
        soundEffects["playerDeath"] = playerDeathSound;
        soundEffects["explosionEnemy"] = explosionEnemySound;
        soundEffects["gameStart"] = levelStartSound;
        soundEffects["driveBy"] = driveBySound;
    }

    /**
     * Toggles the sound to Owen Wilson
     */
    public void Wow()
    {
        if ((AudioClip)soundEffects["explosionEnemy"] != owenWilsonBoom)
        {
            soundEffects["explosionEnemy"] = owenWilsonBoom;
            soundEffects["gameStart"] = owenWilsonStart;
            PlaySingle("gameStart");
        }
        else
            InitSounds();
    }

    /**
     * Toggles the sound to Flex Tape
     */
    public void Flex()
    {
        if ((AudioClip)soundEffects["explosionEnemy"] != majorDamage)
        {
            soundEffects["explosionEnemy"] = majorDamage;
            soundEffects["gameStart"] = philStartGame;
            soundEffects["explosionPlayer"] = philEndGame;

            efxSource.clip = philStart;
            efxSource.Play();
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

        if(!efxSource.isPlaying) // If primary efxSource is not busy
        {
            //Set the clip of our efxSource audio source to the clip passed in as a parameter.
            efxSource.clip = clip;

            //Play the clip.
            efxSource.Play();
        }
        else if (!secondaryEfxSource.isPlaying) // If secondary efxSource is not busy
        {
            //Set the clip of our efxSource audio source to the clip passed in as a parameter.
            secondaryEfxSource.clip = clip;

            //Play the clip.
            secondaryEfxSource.Play();
        }
        else if (!tertiaryEfxSource.isPlaying) // If tertiary efxSource is not busy
        {
            //Set the clip of our efxSource audio source to the clip passed in as a parameter.
            tertiaryEfxSource.clip = clip;

            //Play the clip.
            tertiaryEfxSource.Play();
        }
        else // If nothing else works, just play on quaternary efxSource
        {
            //Set the clip of our efxSource audio source to the clip passed in as a parameter.
            quaternaryEfxSource.clip = clip;

            //Play the clip.
            quaternaryEfxSource.Play();
        }
    }

    /**
     * Stops all sound effects
     */
    public void StopSounds()
    {
        efxSource.Stop();
        secondaryEfxSource.Stop();
        tertiaryEfxSource.Stop();
        quaternaryEfxSource.Stop();
    }

    /**
     * Sets the background music of the game
     * @param music The music to be switched to
     * @param crossfade if true, crosssfade the songs. If false, wait for the other music to finish
     * @param shouldWait whether or not the song should wait for the previous song to end before starting
     *          Only used when crossfade is true
     */
    public void SetBackgroundMusic(AudioClip music, bool crossfade, bool shouldWait)
    {
        if(musicSource.isPlaying) // If the musicSource object is the one currently making sound
        {
            secondaryMusicPlaying = false;

            // Set the background music
            secondaryMusicSource.clip = music;

            if(crossfade)
            {
                InvokeRepeating("SwitchMusic", 0f, 0.01f); // Begin the cross-fade
                secondaryMusicSource.volume = 0;
                secondaryMusicSource.Play();
            }
            else
            {
                if(shouldWait)
                    waitForMusic = true;
                else
                {
                    musicSource.Stop();
                    secondaryMusicSource.Play();
                }
            }
        }
        else
        {
            secondaryMusicPlaying = true;

            // Set the background music
            musicSource.clip = music;

            if (crossfade)
            {
                InvokeRepeating("SwitchMusic", 0f, 0.01f); // Begin the cross-fade
                musicSource.volume = 0;
                musicSource.Play();
            }
            else
            {
                if (shouldWait)
                    waitForMusic = true;
                else
                {
                    secondaryMusicSource.Stop();
                    musicSource.Play();
                }
            }
        }
    }

    /**
     * Waits for the current song to end
     */
    void Update()
    {
        if(waitForMusic) // If the audio source should be waiting for the music to end
        {
            (secondaryMusicPlaying ? secondaryMusicSource : musicSource).loop = false; // Stop looping so it can end
            if(!(secondaryMusicPlaying ? secondaryMusicSource.isPlaying : musicSource.isPlaying)) // If it has stopped
            {
                // Start playing
                if (!secondaryMusicPlaying)
                {
                    secondaryMusicSource.volume = 1f;
                    secondaryMusicSource.Play();
                }
                else
                {
                    musicSource.volume = 1f;
                    musicSource.Play();
                }
                
                waitForMusic = false;
                (secondaryMusicPlaying ? secondaryMusicSource : musicSource).loop = true; // Set the loop to true for the next song
            }
        }
    }

    /**
     * Cross-fades between the two music tracks
     */
    void SwitchMusic()
    {
        if (!secondaryMusicPlaying) // If the musicSource object is the one playing currently
        {
            if (secondaryMusicSource.volume < 1f)
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
            if (musicSource.volume < 1f)
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

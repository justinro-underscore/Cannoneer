using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeManager : MonoBehaviour
{
    public GameObject scoreIncreaseText; // Prefab for showing the cheat was activated

    // Changes explosion sound effects to Owen Wilson saying "Wow"
    private bool owenMode;
    private int owenCode;

    // Activates God Mode
    private bool konamiMode;
    private int konamiCode;

    bool codesAllowed; // Whether or not cheat codes are allowed

    /**
     * Initialize the variables
     */
	void Start ()
    {
        codesAllowed = true;
        owenMode = false;
        owenCode = 0;
        konamiMode = false;
        konamiCode = 0;
	}
	
    /**
     * Input the codes
     */
	void Update ()
    {
        if (codesAllowed)
        {
            // UP
            if (Input.GetKeyDown("up"))
            {
                if (konamiCode == 0 || konamiCode == 1)
                    konamiCode++;
                else
                    konamiCode = 0;

                if (owenCode == 0 || owenCode == 1)
                    owenCode++;
                else
                    owenCode = 0;
            }

            // DOWN
            if (Input.GetKeyDown("down"))
            {
                if (konamiCode == 2 || konamiCode == 3)
                    konamiCode++;
                else
                    konamiCode = 0;

                if (owenCode == 3 || owenCode == 4)
                    owenCode++;
                else
                    owenCode = 0;
            }

            // LEFT
            if (Input.GetKeyDown("left"))
            {
                if (konamiCode == 4 || konamiCode == 6)
                    konamiCode++;
                else
                    konamiCode = 0;
            }

            // RIGHT
            if (Input.GetKeyDown("right"))
            {
                if (konamiCode == 5 || konamiCode == 7)
                    konamiCode++;
                else
                    konamiCode = 0;
            }

            // A BUTTON
            if (Input.GetKeyDown("o"))
            {
                if (konamiCode == 9)
                    konamiCode++;
                else
                    konamiCode = 0;

                if (owenCode == 2 || owenCode == 6)
                    owenCode++;
                else
                    owenCode = 0;
            }

            // B BUTTON
            if (Input.GetKeyDown("l"))
            {
                if (konamiCode == 8)
                    konamiCode++;
                else
                    konamiCode = 0;

                if (owenCode == 5 || owenCode == 7)
                    owenCode++;
                else
                    owenCode = 0;
            }

            // ENTER BUTTON
            if (Input.GetKeyDown("return"))
            {
                if (konamiCode == 10)
                    konamiCode++;
                else
                    konamiCode = 0;

                if (owenCode == 8)
                    owenCode++;
                else
                    owenCode = 0;
            }

            CheckCodeComplete(); // Check if any codes were completed
        }
	}

    /**
     * Checks to see if any codes were completed
     * If they were, activate the code
     */
    void CheckCodeComplete()
    {
        // Owen Wilson Mode
        if (owenCode == 9)
        {
            GameObject showCheat = Instantiate(scoreIncreaseText, GameObject.FindObjectOfType<Canvas>().transform);
            owenMode = !owenMode;
            if (owenMode)
                (showCheat.GetComponent<ScoreIncreaseTextController>() as ScoreIncreaseTextController).SetParamsForCheat("Owen Wilson Mode Activated!");
            else
                (showCheat.GetComponent<ScoreIncreaseTextController>() as ScoreIncreaseTextController).SetParamsForCheat("Owen Wilson Mode Deactivated!");

            SoundManager.instance.Wow(); // Activate cheat

            owenCode = 0;
        }

        // God Mode
        if (konamiCode == 11)
        {
            GameObject showCheat = Instantiate(scoreIncreaseText, GameObject.FindObjectOfType<Canvas>().transform);

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) // If the user is currently playing
            {
                konamiMode = !konamiMode;
                if (konamiMode)
                    (showCheat.GetComponent<ScoreIncreaseTextController>() as ScoreIncreaseTextController).SetParamsForCheat("God Mode Activated!");
                else
                    (showCheat.GetComponent<ScoreIncreaseTextController>() as ScoreIncreaseTextController).SetParamsForCheat("God Mode Deactivated!");

                (player.GetComponent<PlayerController>() as PlayerController).ToggleDebugMode();
            }
            else // If the user isn't currently playing
            {
                (showCheat.GetComponent<ScoreIncreaseTextController>() as ScoreIncreaseTextController).SetParamsForCheat("Must Be Playing to\nActivate Cheat!");
            }

            konamiCode = 0;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeManager : MonoBehaviour
{
    public GameObject scoreIncreaseText; // Prefab for showing the cheat was activated

    private bool owenMode;
    private int owenCode;

    private bool konamiMode;
    private int konamiCode;

    bool codesAllowed;

	// Use this for initialization
	void Start ()
    {
        codesAllowed = false;
        owenMode = false;
        owenCode = 0;
        konamiMode = false;
        konamiCode = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetKeyDown("up"))
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
        if (Input.GetKeyDown("left"))
        {
            if (konamiCode == 4 || konamiCode == 6)
                konamiCode++;
            else
                konamiCode = 0;
        }
        if (Input.GetKeyDown("right"))
        {
            if (konamiCode == 5 || konamiCode == 7)
                konamiCode++;
            else
                konamiCode = 0;
        }
        if (Input.GetKeyDown("o")) // A
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
        if (Input.GetKeyDown("l")) // B
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

        CheckCodeComplete();
	}

    void CheckCodeComplete()
    {
        if (owenCode == 9)
        {
            GameObject showCheat = Instantiate(scoreIncreaseText, GameObject.FindObjectOfType<Canvas>().transform);
            owenMode = !owenMode;
            if(owenMode)
                (showCheat.GetComponent<ScoreIncreaseTextController>() as ScoreIncreaseTextController).SetParamsForCheat("Owen Wilson Mode Activated!");
            else
                (showCheat.GetComponent<ScoreIncreaseTextController>() as ScoreIncreaseTextController).SetParamsForCheat("Owen Wilson Mode Deactivated!");
            
            SoundManager.instance.Wow(); // Activate cheat

            owenCode = 0;
        }
        if (konamiCode == 11)
        {
            GameObject showCheat = Instantiate(scoreIncreaseText, GameObject.FindObjectOfType<Canvas>().transform);

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                
                konamiMode = !konamiMode;
                if (konamiMode)
                    (showCheat.GetComponent<ScoreIncreaseTextController>() as ScoreIncreaseTextController).SetParamsForCheat("God Mode Activated!");
                else
                    (showCheat.GetComponent<ScoreIncreaseTextController>() as ScoreIncreaseTextController).SetParamsForCheat("God Mode Deactivated!");

                (player.GetComponent<PlayerController>() as PlayerController).ToggleDebugMode();
            }
            else
            {
                (showCheat.GetComponent<ScoreIncreaseTextController>() as ScoreIncreaseTextController).SetParamsForCheat("Must Be Playing to\nActivate Cheat!");
            }

            konamiCode = 0;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager instance = null; // So we can access this outside

    public GameObject highScoreObjects; // References to all the text and continue button
    public GameObject initialsBtns; // References to all the initials buttons
    private Text highScoreText; // The score telling the user they got a high score
    private Text initialsText; // The text where the user inputs their initials
    private Text[] initialsObj;

    private int[] initials; // The initials of the player
    private int initialIndex; // The index of the current letter being changed
    private bool cursorBlink; // Whether or not the letter is bold

    /**
     * Starts the game, runs at the start
     */
    void Awake()
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

        // Initialize the initials to "AAA"
        initials = new int[3];
        initials[0] = 'A';
        initials[1] = 'A';
        initials[2] = 'A';
        initialIndex = 0;
        cursorBlink = false;

        // Set up the references to game objects
        highScoreText = highScoreObjects.GetComponentsInChildren<Text>()[1];
        initialsText = highScoreObjects.GetComponentsInChildren<Text>()[0];
        initialsObj = new Text[3];
        for (int i = 0; i < initialsObj.Length; i++)
        {
            initialsObj[i] = highScoreObjects.GetComponentsInChildren<Text>()[2 + i];
        }
    }

    /**
     * Starts the HighScoreManager's functions
     */
    public void ShowHighScore(int score)
    {
        highScoreObjects.SetActive(true);
        initialsBtns.SetActive(true);
        highScoreText.text = "<b>Congratulations! New High Score: " + score + "</b>";
        initialsText.text = "Enter Initials:"; // Show the text
        InvokeRepeating("CursorBlinkToggle", 0.5f, 0.5f);
    }

    /**
     * Updates the initial from the initials buttons
     * @param str Index and up boolean in string form ("index true/false")
     *   Index tells which initial to update
     *   Up boolean tells, if true, to change the letter forward, backward otherwise
     */
    public void UpdateInitial(string str)
    {
        int index = int.Parse(str.Substring(0, 1));
        bool up = bool.Parse(str.Substring(2));
        initials[index] += (up ? 1 : -1);
        if (up && initials[index] > 'Z') // Wrap down
            initials[index] = 'A';
        else if (!up && initials[index] < 'A') // Wrap up
            initials[index] = 'Z';

        // Update the UI
        if (cursorBlink)
            initialsObj[index].text = "<b>" + ((char)initials[index]) + "</b>";
        else
            initialsObj[index].text = "" + ((char)initials[index]);
    }

    /**
     * Toggles the cursorBlink
     */
    void CursorBlinkToggle()
    {
        cursorBlink = !cursorBlink; // Shows whether or not the selected letter should be bold
        for (int i = 0; i < initialsObj.Length; i++)
        {
            if (cursorBlink)
                initialsObj[i].text = "<b>" + ((char)initials[i]) + "</b>";
            else
                initialsObj[i].text = "" + ((char)initials[i]);
        }
    }

    /**
     * Sets the cursor blink to true, only used when changing something
     */
    void SetCursorBlinkTrue()
    {
        CancelInvoke("CursorBlinkToggle"); // So we can reset the invoke
        cursorBlink = true;
        InvokeRepeating("CursorBlinkToggle", 0.5f, 0.5f);
    }

    /**
     * Returns the initials
     * @param internalCall Whether or not the call was internal or external
     *  if internal, blink the letters & add spaces
     * @return Returns the initials
     */
    public string GetInitials(bool internalCall)
    {
        // This is very messy... I'm sorry
        string result = "";
        // The first initial
        if (internalCall && cursorBlink && initialIndex == 0)
            result += "<b>" + ((char)initials[0]) + "</b>";
        else
            result += ((char)initials[0]);

        if (internalCall)
            result += " ";

        // The second initial
        if (internalCall && cursorBlink && initialIndex == 1)
            result += "<b>" + ((char)initials[1]) + "</b>";
        else
            result += ((char)initials[1]);

        if (internalCall)
            result += " ";

        // The third initial
        if (internalCall && cursorBlink && initialIndex == 2)
            result += "<b>" + ((char)initials[2]) + "</b>";
        else
            result += ((char)initials[2]);

        return result;
    }

    /**
     * Resets the high score manager
     */
    public void Reset()
    {
        CancelInvoke("CursorBlinkToggle");
        highScoreText.text = "";
        initialsText.text = "";
        highScoreObjects.SetActive(false);
        initialsBtns.SetActive(false);
    }
}

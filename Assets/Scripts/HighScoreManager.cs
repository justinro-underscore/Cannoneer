using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager instance = null; // So we can access this outside

    public Text highScoreText; // The score telling the user they got a high score
    public Text initialsText; // The text where the user inputs their initials

    private bool running; // Whether or not the high score manager is running

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

        running = false; // Will start running when GameManager calls it

        // Initialize the initials to "AAA"
        initials = new int[3];
        initials[0] = 'A';
        initials[1] = 'A';
        initials[2] = 'A';
        initialIndex = 0;
        cursorBlink = false;
    }

    /**
     * Starts the HighScoreManager's functions
     */
    public void ShowHighScore(int score)
    {
        running = true;
        highScoreText.text = "<b>Congratulations! New High Score: " + score + "</b>";
        InvokeRepeating("CursorBlinkToggle", 0f, 0.5f); // Blinks the current initials
    }

    /**
     * Prints the text and moves the cursor
     */
    void Update ()
    {
        if (running)
        {
            initialsText.text = "Enter Initials: " + GetInitials(true); // Show the text

            // Change the initial
            if (Input.GetKeyDown("up"))
                ChangeCharacter(true);
            else if (Input.GetKeyDown("down"))
                ChangeCharacter(false);
            // Change the current initial selected
            else if (Input.GetKeyDown("right"))
                MoveCursor(true);
            else if (Input.GetKeyDown("left"))
                MoveCursor(false);
        }
    }

    /**
     * Toggles the cursorBlink
     */
    void CursorBlinkToggle()
    {
        cursorBlink = !cursorBlink; // Shows whether or not the selected letter should be bold
    }

    /**
     * Sets the cursor blink to true, only used when changing something
     */
    void SetCursorBlinkTrue()
    {
        CancelInvoke(); // So we can reset the invoke
        cursorBlink = true;
        InvokeRepeating("CursorBlinkToggle", 0.5f, 0.5f);
    }

    /**
     * Changes the currently selected initial's character
     * @param up Whether the current initial should change its ascii value up the alphabet or down
     */
    void ChangeCharacter(bool up)
    {
        initials[initialIndex] = initials[initialIndex] + (up ? -1 : 1); // Changes the initial
        if (initials[initialIndex] > 'Z') // Wraps
            initials[initialIndex] = 'A';
        else if (initials[initialIndex] < 'A') // Wraps
            initials[initialIndex] = 'Z';
        SetCursorBlinkTrue(); // Reset the cursor blink
    }

    /**
     * Moves the cursor to another character
     * @param Whether the cursor should go right or left
     */
    void MoveCursor(bool right)
    {
        initialIndex += (right ? 1 : -1); // Changes the selected initial
        if (initialIndex > 2) // Wraps
            initialIndex = 0;
        else if (initialIndex < 0) // Wraps
            initialIndex = 2;
        SetCursorBlinkTrue(); // Reset the cursor blink
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
        running = false;
        CancelInvoke();
        highScoreText.text = "";
        initialsText.text = "";
        initials[0] = 'A'; // Reset initials
        initials[1] = 'A';
        initials[2] = 'A';
    }
}

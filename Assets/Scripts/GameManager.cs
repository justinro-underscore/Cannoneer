using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    enum State
    {
        MAIN_MENU, // The main menu (splash screen)
        GAME, // The game, running
        ENDSCREEN, // When the player finishes the level
        LEADERBOARD // The leaderboard after player loses
    }
    private State currState = State.MAIN_MENU; // The current state the game is in

    private string[] leaderboardNames; // Names of the top 10 scorers
    private int[] leaderboardScores; // Scores of the top 10 scorers

    public static GameManager instance = null; // So this instance can be used in other classes
    public GameObject levelManager; // The LevelManager prefab
    public Text scoreText; // Text showing player score
    public Text levelOverText; // Text showing level over text
    public Text leaderboardText; // Text showing the leaderboard

    private GameObject player; // So we can enable and disable the player

    private int playerScore; // Player's current score
    private int level; // Current level

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

        // Reset the player
        player = GameObject.FindGameObjectWithTag("Player");
        player.SetActive(false);

        // Populate the leaderboard
        leaderboardNames = new string[10];
        leaderboardScores = new int[10];
        PopulateLeaderboard();

        // Show the menu
        ShowMenu();
    }

    void PopulateLeaderboard()
    {
        for(int i = 0; i < 10; i++)
        {
            leaderboardNames[i] = "AAA";
            leaderboardScores[i] = 0;
        }
        // TODO Use file I/O to populate this
        // TODO Figure out how to do onQuit, write to file
    }

    /**
     * Shows the main menu of the game
     */
    void ShowMenu()
    {
        currState = State.MAIN_MENU;

        leaderboardText.text = ""; // Just make sure this is empty
        levelOverText.text = "Welcome to Cannoneer!\nPress 'Enter' to start!";
    }

    /**
     * Check to see when to start the game
     */
    void Update()
    {
        if (currState == State.MAIN_MENU && Input.GetKeyDown(KeyCode.Return))
            InitGame();
    }

    /**
     * Initialize the new game
     */
    void InitGame()
    {
        currState = State.GAME;

        levelOverText.text = "";

        // Set the starting values
        playerScore = 0;
        level = 1;

        InitLevel();
    }

    /**
     * Initialize the level
     */
    void InitLevel()
    {
        player.SetActive(true);
        (player.GetComponent<PlayerController>() as PlayerController).Restart();

        IncreaseScore(0); // Do 0 so we can display the score

        //Call the InitGame function to initialize the first level
        Instantiate(levelManager);
        LevelManager.instance.SetLevel(level);
    }
    
    /**
     * Increases player score
     * @param score The amount to increase score by
     */
    public void IncreaseScore(int score)
    {
        playerScore += score;
        scoreText.text = "Score: " + playerScore;
    }

    /**
     * What to show when the player dies
     */
    public void GameOver()
    {
        levelOverText.text = "Game Over";
        scoreText.text = "";
        (player.GetComponent<PlayerController>() as PlayerController).MovePlayer(new Vector2(0, -2)); // Put the player underneath the text
        Invoke("ShowLeaderboard", 4f);
    }

    /**
     * Shows the leaderboard of the game, after the player has lost
     */
    void ShowLeaderboard()
    {
        currState = State.LEADERBOARD;

        player.SetActive(false); // Make sure you can't see the user
        levelOverText.text = "Your score: " + playerScore + "\n\n\n\n\n"; // All of the new lines are to format the score
        for(int i = 0; i < 10; i++)
        {
            leaderboardText.text += leaderboardNames[i] + "............" + string.Format("{0:000000}", leaderboardScores[i]) + "\n"; // Show other scores
        }
        Invoke("ShowMenu", 10f);
    }
}

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
        ENDGAME, // When the player loses
        LEADERBOARD // The leaderboard after player loses
    }
    private State currState = State.LEADERBOARD; // The current state the game is in (starts in leaderboard so that we can call ShowMenu())

    private string[] leaderboardNames; // Names of the top 10 scorers
    private int[] leaderboardScores; // Scores of the top 10 scorers

    public static GameManager instance = null; // So this instance can be used in other classes
    public GameObject levelManager; // The LevelManager prefab
    public Text scoreText; // Text showing player score
    public Text levelOverText; // Text showing level over text
    public Text leaderboardText; // Text showing the leaderboard

    // Handles showing stats at the end of the level
    public Text levelText;
    public Text shipsText;
    public Text treasureText;
    public Text obstaclesText;
    public int shipsScore; // Amount of points for destroying ships
    public int treasureScore; // Amount of points for collecting treasure
    public int obstaclesScore; // Amount of points for destroying obstacles

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
        if (currState != State.MAIN_MENU && currState != State.GAME) // Make sure we don't call this after we start the game
        {
            currState = State.MAIN_MENU;

            leaderboardText.text = ""; // Just make sure this is empty
            levelOverText.text = "Welcome to Cannoneer!\nPress 'Enter' to start!";
        }
    }

    /**
     * Check to see when to start the game
     */
    void Update()
    {
        if (currState == State.MAIN_MENU && Input.GetKeyDown(KeyCode.Return))
            InitGame();
        if (currState == State.LEADERBOARD && Input.GetKeyDown(KeyCode.Return))
            ShowMenu();
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

        // Set initial values
        shipsScore = 0;
        treasureScore = 0;
        obstaclesScore = 0;
        IncreaseScore(0, ""); // Do 0 so we can display the score

        //Call the InitGame function to initialize the first level
        Instantiate(levelManager);
        LevelManager.instance.SetLevel(level);
    }
    
    /**
     * Increases player score
     * @param score The amount to increase score by
     */
    public void IncreaseScore(int score, string objectName)
    {
        playerScore += score;
        scoreText.text = "Score: " + playerScore;

        // Adds the score to respecitve category
        switch(objectName)
        {
            case "ship":
                shipsScore += score;
                break;
            case "treasure":
                treasureScore += score;
                break;
            case "obstacle":
                obstaclesScore += score;
                break;
            default:
                break;
        }
    }

    /**
     * Shows the stats that the player got for that level
     */
    public void ShowLevelStats()
    {
        (player.GetComponent<PlayerController>() as PlayerController).ToggleIsDead(); // Makes it so that the player cannot move or shoot
        (player.GetComponent<PlayerController>() as PlayerController).MovePlayer(new Vector2(-8, 0)); // Move the player

        levelText.text = "Level " + level + " Complete!";
        InvokeRepeating("ShowShipsScore", 1f, 0.01f); // For the count up
    }

    /**
     * Shows the amount of points recieved for destroying ships
     */
    void ShowShipsScore()
    {
        // So we don't need a global variable to count up
        int currScore = 0;
        int index = shipsText.text.IndexOfAny("123456789".ToCharArray());
        if(index != -1)
            System.Int32.TryParse(shipsText.text.Substring(index), out currScore);

        // Set the score
        if (currScore < shipsScore)
        {
            currScore += 2; // Count up
            shipsText.text = "Ships Destroyed..................." + string.Format("{0:00000}", currScore); // TODO TAKE OUT 0s
        }
        else if (shipsScore == 0) // If there was no points received here...
        {
            shipsText.text = "Ships Destroyed..................." + string.Format("{0:00000}", currScore); // Show the text with 0
            shipsScore--; // So we don't accidentally trigger this a second time (this will be reset at the start of the next level)
        }
        else
        {
            CancelInvoke(); // Stop counting up
            InvokeRepeating("ShowTreasureScore", 1f, 0.01f); // Start counting the next category
        }
    }

    /**
     * Shows the amount of points recieved for collecting treasure
     */
    void ShowTreasureScore()
    {
        int currScore = 0;
        int index = treasureText.text.IndexOfAny("123456789".ToCharArray());
        if (index != -1)
            System.Int32.TryParse(treasureText.text.Substring(index), out currScore);
        if (currScore < treasureScore)
        {
            currScore += 2;
            treasureText.text = "Treasure Collected................" + string.Format("{0:00000}", currScore);
        }
        else if (treasureScore == 0)
        {
            treasureText.text = "Treasure Collected................" + string.Format("{0:00000}", currScore);
            treasureScore--;
        }
        else
        {
            CancelInvoke();
            InvokeRepeating("ShowObstaclesText", 1f, 0.01f);
        }
    }

    /**
     * Shows the amount of points recieved for destroying obstacles
     */
    void ShowObstaclesText()
    {
        int currScore = 0;
        int index = obstaclesText.text.IndexOfAny("123456789".ToCharArray());
        if (index != -1)
            System.Int32.TryParse(obstaclesText.text.Substring(index), out currScore);
        if (currScore < obstaclesScore)
        {
            currScore += 2;
            obstaclesText.text = "Obstacles Destroyed..............." + string.Format("{0:00000}", currScore);
        }
        else if (obstaclesScore == 0)
        {
            obstaclesText.text = "Obstacles Destroyed..............." + string.Format("{0:00000}", currScore);
            obstaclesScore--;
        }
        else
        {
            CancelInvoke();
            // TODO Move on to next level
        }
    }

    /**
     * What to show when the player dies
     */
    public void GameOver()
    {
        currState = State.ENDGAME;
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
        for (int i = 0; i < 10; i++)
        {
            leaderboardText.text += leaderboardNames[i] + "............" + string.Format("{0:000000}", leaderboardScores[i]) + "\n"; // Show other scores
        }
        Invoke("ShowMenu", 10f);
    }
}

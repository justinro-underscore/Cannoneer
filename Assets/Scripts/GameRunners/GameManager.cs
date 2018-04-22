using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum State
    {
        MAIN_MENU, // The main menu (splash screen)
        GAME, // The game, running
        ENDSCREEN, // When the player finishes the level
        ENDGAME, // When the player loses
        INITIALS, // If the player gets a high score
        LEADERBOARD // The leaderboard after player loses
    }
    private State currState = State.LEADERBOARD; // The current state the game is in (starts in leaderboard so that we can call ShowMenu())

    // The level specific spawning rates of different ships (sloop, brigantine, frigate, runner)
    private static float[,] levelSpawningRates = new float[10, 4] {
        { 1f, 0f, 0f, 0f }, // Level 1
        { 0.9f, 0.1f, 0f, 0f }, // Level 2
        { 0.8f, 0.2f, 0f, 0f }, // Level 3
        { 0.7f, 0.2f, 0.1f, 0f }, // Level 4
        { 0.6f, 0.25f, 0.15f, 0f }, // Level 5
        { 0.5f, 0.2f, 0.2f, 0.1f }, // Level 6
        { 0.4f, 0.15f, 0.3f, 0.15f }, // Level 7
        { 0.3f, 0.15f, 0.35f, 0.2f }, // Level 8
        { 0.2f, 0.15f, 0.4f, 0.25f }, // Level 9
        { 0.1f, 0.2f, 0.45f, 0.25f } }; // Level 10 +

    public AudioClip gameMusic; // Background music of the game
    public AudioClip menuMusic; // Background music of the menu

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
    public Text levelCompScoreText; // Shows the score achieved by completing the level
    public int shipsScore; // Amount of points for destroying ships
    public int treasureScore; // Amount of points for collecting treasure
    public int obstaclesScore; // Amount of points for destroying obstacles

    private GameObject player; // So we can enable and disable the player

    private int playerScore; // Player's current score
    private string playerName; // Used for the leaderboard
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
        LoadLeaderboard();

        // Show the menu
        ShowMenu();
    }

    /**
     * Shows the main menu of the game
     */
    void ShowMenu()
    {
        CancelInvoke("ShowMenu");
        currState = State.MAIN_MENU;

        SoundManager.instance.SetBackgroundMusic(menuMusic);

        leaderboardText.text = ""; // Just make sure this is empty
        levelOverText.text = "Welcome to Cannoneer!\nPress 'Enter' to start!";
    }

    /**
     * Check to see when to start the game
     */
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (currState == State.MAIN_MENU && Input.GetKeyDown(KeyCode.Return))
            InitGame();
        if (currState == State.INITIALS && Input.GetKeyDown(KeyCode.Return))
            SubmitInitials();
        if (currState == State.LEADERBOARD && Input.GetKeyDown(KeyCode.Return))
            ShowMenu();
    }

    /**
     * Initialize the new game
     */
    void InitGame()
    {
        levelOverText.text = "";

        // Set the starting values
        playerScore = 0;
        playerName = "AAA";
        level = 5;

        SoundManager.instance.SetBackgroundMusic(gameMusic);
        SoundManager.instance.PlaySingle("gameStart");

        InitLevel();
    }

    /**
     * Initialize the level
     */
    void InitLevel()
    {
        currState = State.GAME;

        // Reset the stats text
        levelText.text = "";
        shipsText.text = "";
        treasureText.text = "";
        obstaclesText.text = "";
        levelCompScoreText.text = "";

        level++;
        ToggleLevelText();

        // Reset the player
        player.SetActive(true);
        (player.GetComponent<PlayerController>() as PlayerController).Restart();

        // Set initial values
        shipsScore = 0;
        treasureScore = 0;
        obstaclesScore = 0;
        IncreaseScore(0, ""); // Do 0 so we can display the score

        //Call the InitGame function to initialize the level
        Instantiate(levelManager);
        LevelManager.instance.SetLevel(level);
    }

    /**
     * Returns the spawning rates for the current level
     */
    public float[] GetSpawningRates()
    {
        int currLevel = (level < 10 ? level - 1 : 9); // If level 10 or later, juse use level 10's rates
        float[] result = new float[4];
        for(int i = 0; i < 4; i++)
        {
            result[i] = 0; // Make sure we don't try and access an undefined reference
            for(int j = 0; j <= i; j++)
            {
                result[i] += levelSpawningRates[currLevel, j]; // Copy the values
            }
        }
        return result;
    }

    /**
     * Shows the beginning level text
     */
    void ToggleLevelText()
    {
        if (levelOverText.text.Equals(""))
        {
            levelOverText.text = "Stage " + level;
            Invoke("ToggleLevelText", 2.5f); // Toggle off after 2.5 seconds
        }
        else
            levelOverText.text = "";
    }
    
    /**
     * Increases player score
     * @param score The amount to increase score by
     */
    public void IncreaseScore(int score, string objectName)
    {
        if(!(player.GetComponent<PlayerController>() as PlayerController).GetDebugMode())
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
        currState = State.ENDSCREEN;
        Destroy(LevelManager.instance.gameObject); // End the level
        (player.GetComponent<PlayerController>() as PlayerController).ToggleIsDead(); // Makes it so that the player cannot move or shoot
        (player.GetComponent<PlayerController>() as PlayerController).MovePlayer(new Vector2(-8, 0)); // Move the player

        IncreaseScore(200 * level, ""); // Level complete score
        levelText.text = "Stage " + level + " Complete!";
        InvokeRepeating("ShowLevelCompleteScoreText", 1f, 0.01f); // For the count up
    }

    /**
     * Shows the amount of points recieved for completing the level
     */
    void ShowLevelCompleteScoreText()
    {
        // So we don't need a global variable to count up
        int currScore = 0;
        int index = levelCompScoreText.text.IndexOfAny("123456789".ToCharArray());
        if (index != -1)
            System.Int32.TryParse(levelCompScoreText.text.Substring(index), out currScore);

        // Set the score
        if (currScore < level * 200)
        {
            currScore += 5; // Count up
            levelCompScoreText.text = "Stage Completed..................." + string.Format("{0:00000}", currScore); // TODO TAKE OUT 0s
        }
        else
        {
            CancelInvoke("ShowLevelCompleteScoreText"); // Stop counting up
            InvokeRepeating("ShowShipsScore", 1f, 0.01f); // Start counting the next category
        }
    }

    /**
     * Shows the amount of points recieved for destroying ships
     */
    void ShowShipsScore()
    {
        int currScore = 0;
        int index = shipsText.text.IndexOfAny("123456789".ToCharArray());
        if(index != -1)
            System.Int32.TryParse(shipsText.text.Substring(index), out currScore);
        if (currScore < shipsScore)
        {
            currScore += 5;
            shipsText.text = "Ships Destroyed..................." + string.Format("{0:00000}", currScore);
        }
        else if (shipsScore == 0) // If there was no points received here...
        {
            shipsText.text = "Ships Destroyed..................." + string.Format("{0:00000}", currScore); // Show the text with 0
            shipsScore--; // So we don't accidentally trigger this a second time (this will be reset at the start of the next level)
        }
        else
        {
            CancelInvoke("ShowShipsScore");
            InvokeRepeating("ShowTreasureScore", 1f, 0.01f);
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
            currScore += 5;
            treasureText.text = "Treasure Collected................" + string.Format("{0:00000}", currScore);
        }
        else if (treasureScore == 0)
        {
            treasureText.text = "Treasure Collected................" + string.Format("{0:00000}", currScore);
            treasureScore--;
        }
        else
        {
            CancelInvoke("ShowTreasureScore");
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
            currScore += 5;
            obstaclesText.text = "Obstacles Destroyed..............." + string.Format("{0:00000}", currScore);
        }
        else if (obstaclesScore == 0)
        {
            obstaclesText.text = "Obstacles Destroyed..............." + string.Format("{0:00000}", currScore);
            obstaclesScore--;
        }
        else
        {
            CancelInvoke("ShowObstaclesText");
            Invoke("InitLevel", 4f); // Move on to next level
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
        Destroy(LevelManager.instance.gameObject); // Get rid of the level manager
        Invoke("ShowInitials", 4f);
    }

    /**
     * Restarts the current level
     */
    public void RestartLevel()
    {
        Destroy(LevelManager.instance.gameObject); // Get rid of the level manager
        level--; // Redo level
        playerScore -= (shipsScore + obstaclesScore + treasureScore); // Get rid of all progress
        SoundManager.instance.SetBackgroundMusic(gameMusic);
        Invoke("InitLevel", 0.05f); // So we don't accidentally delete the level manager again
    }

    /**
     * Shows the Enter Initials screen if player got a high score, leaderboard if not
     */
    void ShowInitials()
    {
        if (playerScore > leaderboardScores[9]) // If the player gets a higher score than the last high score
        {
            levelOverText.text = "";
            currState = State.INITIALS;
            HighScoreManager.instance.ShowHighScore(playerScore); // Change the screen
        }
        else
            ShowLeaderboard();
    }
    
    /**
     * Runs when the user finishes entering their initials (if they achieve a high score)
     */
    void SubmitInitials()
    {
        playerName = HighScoreManager.instance.GetInitials(false);
        HighScoreManager.instance.Reset();
        Invoke("ShowLeaderboard", 0.1f);
    }

    /**
     * Shows the leaderboard of the game, after the player has lost
     */
    void ShowLeaderboard()
    {
        currState = State.LEADERBOARD;

        // Adapted from https://answers.unity.com/questions/958370/how-to-change-alpha-of-a-sprite.html
        player.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f); // Make player transparent
        player.SetActive(false);

        levelOverText.text = "Your score: " + playerScore + "\n\n\n\n\n"; // All of the new lines are to format the score
        int currScore = playerScore;
        string currName = playerName;
        int isScoreOfPlayer = 0; // This is how we know this is the user's score (so we can bold it)
        for (int i = 0; i < 10; i++)
        {
            // Update the scoreboard
            if(currScore > leaderboardScores[i])
            {
                if (isScoreOfPlayer == 0) // This is in place if the player has multiple high scores
                    isScoreOfPlayer++; // This will only be 1 once, the first time through this if statement

                int tempScore = leaderboardScores[i];
                string tempName = leaderboardNames[i];
                leaderboardScores[i] = currScore;
                leaderboardNames[i] = currName;
                currScore = tempScore;
                currName = tempName;
            }
            // Show the scores, make the new high score bold
            leaderboardText.text += (i != 9 ? " " : "") + (i + 1) + ": "; // Add a space if the index is not 10
            leaderboardText.text += (isScoreOfPlayer == 1 ? "<b>" : "") + leaderboardNames[i] + (isScoreOfPlayer == 1 ? "</b>" : "");
            leaderboardText.text += "............";
            leaderboardText.text += (isScoreOfPlayer == 1 ? "<b>" : "") + string.Format("{0:000000}", leaderboardScores[i]) + (isScoreOfPlayer == 1 ? "</b>\n" : "\n");
            if (isScoreOfPlayer == 1) // So this only runs once
                isScoreOfPlayer++;
        }
        Invoke("ShowMenu", 10f);
    }

    /**
     * When player quits, save the leaderboard
     */
    void OnApplicationQuit()
    {
        SaveLeaderboard();
    }

    /**
     * Save the leaderboard to a text file
     */
    void SaveLeaderboard()
    { 
        string path = Application.dataPath + "/Resources/Leaderboard.txt"; // Path where we'll save our file
        // If the file already exists, delete it
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        // Write to the file
        StreamWriter sw = new StreamWriter(path, true);
        for (int i = 0; i < 10; i++)
        {
            sw.WriteLine(leaderboardNames[i] + " " + leaderboardScores[i]); // Write the names
        }
        sw.Close();

        //Re-import the file to update the reference in the editor
        //AssetDatabase.ImportAsset(path);
        //TextAsset asset = (TextAsset)Resources.Load("Leaderboard");
    }

    /**
     * Loads the leaderboard file and saves it
     */
    void LoadLeaderboard()
    {
        string path = Application.dataPath + "/Resources/Leaderboard.txt"; // Path where we'll save our file

        bool defaultLeaderboard = false;
        // If the file already exists, populate leaderboard
        if (File.Exists(path))
        {
            string[] lines = System.IO.File.ReadAllLines(path);
            if (lines.Length >= 10) // Make sure we have info in our file
            {
                for (int i = 0; i < 10; i++)
                {
                    leaderboardNames[i] = lines[i].Substring(0, lines[i].IndexOf(" "));
                    System.Int32.TryParse(lines[i].Substring(lines[i].IndexOf(" ") + 1), out leaderboardScores[i]);
                }
            }
            else // If not, default to starting leaderboard
                defaultLeaderboard = true;
        }
        else // If not, default to starting leaderboard
            defaultLeaderboard = true;

        // Populate the default leaderboard
        if(defaultLeaderboard)
        {
            for (int i = 0; i < 10; i++)
            {
                leaderboardNames[i] = "AAA";
                leaderboardScores[i] = 0;
            }
        }
    }

    /**
     * Returns the current state that the game is in
     */
    public State GetCurrState()
    {
        return currState;
    }
}

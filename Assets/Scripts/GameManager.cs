using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    enum State
    {
        MAIN_MENU,
        GAME,
        ENDSCREEN
    }
    private State currState = State.MAIN_MENU;

    public static GameManager instance = null; // So this instance can be used in other classes
    public GameObject levelManager; // The LevelManager prefab
    public Text scoreText; // Text showing player score
    public Text levelOverText; // Text showing level over text

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

        // Set the starting values
        playerScore = 0;
        level = 1;

        //Call the InitGame function to initialize the first level 
        //LevelManager.instance.InitLevel();
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

    public void GameOver()
    {
        levelOverText.text = "Game Over";
        //currState = State.ENDSCREEN;
    }
}

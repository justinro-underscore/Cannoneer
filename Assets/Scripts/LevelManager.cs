﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance = null; // So this instance can be used in other classes
    public Transform cannonballs; // Holds cannonballs so the heirarchy does not get cluttered
    public Transform ships; // Holds ships so the heirarchy does not get cluttered
    public Transform rocks; // Holds rocks so the heirarchy does not get cluttered
    public Transform treasure; // Holds the treasure so the heirarchy does not get cluttered
    public Text scoreText; // Shows the score of the player
    public Text levelOverText; // Shows game over text

    public GameObject cannonball; // Prefab for Cannonball
    public GameObject sloop; // Prefab for Sloop
    public GameObject broken; // Prefab for Broken Ship
    public GameObject rockSmall; // Prefab for Small Rock
    public GameObject rockLarge; // Prefab for Large Rock
    public GameObject treasureChest; // Prefab for Treasure Chest

    private int playerScore; // Player's current score
    private const int OFFSCREEN = 14; // The number that indicates when something goes offscreen

    /**
     * Starts the level, runs at the start
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

        // Holds the ship objects
        ships = new GameObject("Ships").transform;
        GameObject s = Instantiate(sloop, new Vector3(12, 0), Quaternion.identity); // TODO Not here
        s.transform.SetParent(ships);
        // Holds all the cannonballs created
        cannonballs = new GameObject("Cannonballs").transform;
        // Holds all the rocks created
        rocks = new GameObject("Rocks").transform;
        // Holds all the treasure chests created
        treasure = new GameObject("Treasure").transform;

        playerScore = 0; // Initializes the player score
        scoreText.text = "Score: " + playerScore;

        //Call the InitGame function to initialize the first level 
        InitLevel();
    }

    /**
     * Initializes the level
     */
    void InitLevel()
    {

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
     * Ends the game
     */
    public void EndGame()
    {
        Destroy(ships.gameObject); // Destroys all ships
        Destroy(cannonballs.gameObject); // Destroys all cannonballs
        Destroy(rocks.gameObject); // Destroys all rocks
        Destroy(treasure.gameObject); // Destroys all treasure
        levelOverText.text = "Game Over";
    }
}

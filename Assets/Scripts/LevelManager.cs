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
    private const int OFFSCREEN_X = 14; // The number that indicates when something goes offscreen in x
    private const int OFFSCREEN_Y = 5; // The number that indicates when something goes offscreen in y

    private int level; // Current level

    private int numShipsToDestroy; // The amount of ships to destroy to end the level
    private int numShipsOnScreen; // The number of ships on screen at one time
    private int numShipsDestroyed; // The number of ships that have been destroyed
    private bool levelOver; // Whether or not the level has completed

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

        // Holds the ship objects
        ships = new GameObject("Ships").transform;
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
        // Initialize the member variables
        numShipsOnScreen = 0;
        numShipsDestroyed = 0;
        levelOver = false;

        // TODO Change to a logarithmic function
        // Set the numShipsToDestroy
        if (level == 1)
            numShipsToDestroy = 10;
        else if(level == 2)
            numShipsToDestroy = 15;
        else if(level == 3)
            numShipsToDestroy = 20;

        SpawnObjects();
    }

    /**
     * Spawns all of the objects for the level
     */
    void SpawnObjects()
    {
        // TODO Add functionality for more ships
        float timeTilNextShip = Random.Range(2f, 4f); // TODO Change with the level
        Invoke("CreateSloop", timeTilNextShip);

        float timeTilNextTreasure = Random.Range(5f, 10f);
        Invoke("CreateTreasure", timeTilNextTreasure);

        float timeTilNextRock = Random.Range(1f, 3f);
        Invoke("CreateRock", timeTilNextRock);
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
     * Creates a Sloop ship
     */
    public void CreateSloop()
    {
        if (!levelOver)
        {
            if (numShipsOnScreen <= 2) // The number of ships on the screen should never go more than 3
            {
                GameObject s = Instantiate(sloop, new Vector3(OFFSCREEN_X, Random.Range(-OFFSCREEN_Y + 0.5f, OFFSCREEN_Y - 0.5f)), Quaternion.identity);
                s.transform.SetParent(ships);
                numShipsOnScreen++; // Increase the amount of ships on the screen

                float timeTilNextShip = Random.Range(2f, 4f); // TODO Change with the level
                Invoke("CreateSloop", timeTilNextShip);
            }
            else // Try again later
            {
                float timeTilNextShip = Random.Range(2f, 4f); // TODO Change with the level
                Invoke("CreateSloop", timeTilNextShip);
            }
        }
    }

    /**
     * Creates treasure
     */
    public void CreateTreasure()
    {
        if (!levelOver)
        {
            GameObject t = Instantiate(treasureChest, new Vector3(OFFSCREEN_X, Random.Range(-OFFSCREEN_Y, OFFSCREEN_Y)), Quaternion.identity);
            t.transform.SetParent(treasure);

            float timeTilNextTreasure = Random.Range(5f, 10f);
            Invoke("CreateTreasure", timeTilNextTreasure);
        }
    }

    /**
     * Creates a rock
     */
    public void CreateRock()
    {
        if (!levelOver)
        {
            float rockChoice = Random.value; // Determines which size rock to create
            GameObject r = null;
            if (rockChoice < 0.8) // Chances of getting a small rock are higher than a large rock
                r = Instantiate(rockSmall, new Vector3(OFFSCREEN_X, Random.Range(-OFFSCREEN_Y, OFFSCREEN_Y)), Quaternion.identity);
            else
                r = Instantiate(rockLarge, new Vector3(OFFSCREEN_X, Random.Range(-OFFSCREEN_Y, OFFSCREEN_Y)), Quaternion.identity);
            r.transform.SetParent(rocks);

            float timeTilNextRock = Random.Range(1f, 3f);
            Invoke("CreateRock", timeTilNextRock);
        }
    }

    /**
     * Ends the game
     */
    public void EndGame()
    {
        levelOver = true;
        Destroy(ships.gameObject); // Destroys all ships
        Destroy(cannonballs.gameObject); // Destroys all cannonballs
        Destroy(rocks.gameObject); // Destroys all rocks
        Destroy(treasure.gameObject); // Destroys all treasure
        levelOverText.text = "Game Over";
        Invoke("GameManager.instance.GameOver", 2f);
    }

    /**
     * Ends the level
     */
    void EndLevel()
    {

    }

    /**
     * Removes a ship from numShipsOnScreen & adds to the amt destroyed
     * @param destroyed Whether or not the player destroyed the ship
     */
    public void RemoveShip(bool destroyed)
    {
        numShipsOnScreen--;
        if (destroyed) // If the ship was destroyed...
        {
            numShipsDestroyed++;
            if (numShipsDestroyed == numShipsToDestroy) // If the game should end
            {
                levelOver = true;
                EndLevel();
            }
        }
    }
}

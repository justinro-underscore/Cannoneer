using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance = null; // So this instance can be used in other classes
    public Transform cannonballs; // Holds cannonballs so the heirarchy does not get cluttered
    public Transform ships; // Holds ships so the heirarchy does not get cluttered
    public Transform obstacles; // Holds obstacles so the heirarchy does not get cluttered
    public Transform treasure; // Holds the treasure so the heirarchy does not get cluttered
    
    public GameObject cannonball; // Prefab for Cannonball
    public GameObject sloop; // Prefab for Sloop
    public GameObject broken; // Prefab for Broken Ship
    public GameObject brigantine; // Prefab for Brigantine
    public GameObject rockSmall; // Prefab for Small Rock
    public GameObject rockLarge; // Prefab for Large Rock
    public GameObject shark; // Prefab for Shark
    public GameObject treasureChest; // Prefab for Treasure Chest
    public GameObject scoreIncreaseText; // Prefab for the score increase text
    public Image progressBar; // Prefab for the Progress Bar

    private Image progressBarRef; // Reference to the progress bar
    private RectTransform progressBarRect; // Reference to the RectTransform (used for easy size manipulation)
    private Color progressBarColor; // Color of the progress bar (NOTE: not a reference)
    private int progressBarWidth; // The desired width of the progress bar
    private int progressAmt; // The amount by which the progress bar should increase after each ship destroyed
    private const int PROGRESS_BAR_FULL_WIDTH = 1510; // The size of the full progress bar

    private const int OFFSCREEN_X = 14; // The number that indicates when something goes offscreen in x
    private const int OFFSCREEN_Y = 5; // The number that indicates when something goes offscreen in y

    private int level; // Current level

    private int numShipsToDestroy; // The amount of ships to destroy to end the level
    private int numShipsOnScreen; // The number of ships on screen at one time
    private int numBrigantinesOnScreen; // The number of Brigantines on screen
    private bool levelOver; // Whether or not the level is over
    private bool levelComplete; // Whether or not the level has completed

    private int numShipsDestroyed; // The number of ships that have been destroyed

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

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a LevelManager.
            Destroy(gameObject);

        // Holds the ship objects
        ships = new GameObject("Ships").transform;
        // Holds all the cannonballs created
        cannonballs = new GameObject("Cannonballs").transform;
        // Holds all the obstacles created
        obstacles = new GameObject("Obstacles").transform;
        // Holds all the treasure chests created
        treasure = new GameObject("Treasure").transform;
        
        // Create the progress bar
        Instantiate(progressBar, GameObject.FindObjectOfType<Canvas>().transform);
        // Adapted from https://answers.unity.com/questions/1072456/change-width-of-ui-image-c.html
        progressBarRef = GameObject.FindObjectOfType<Image>();
        progressBarRect = progressBarRef.transform as RectTransform;
        progressBarColor = new Color(1, 1, 1);

        // Call the InitGame function to initialize the level
        // The wait is to have time to set the level
        Invoke("InitLevel", 0.05f);
    }

    /**
     * Updates the progress bar
     */
    void Update()
    {
        if (!levelOver) // Do this unless player dies
        {
            // Increase the size of the progress bar gradually
            if(progressBarRect.sizeDelta.x < progressBarWidth)
                progressBarRect.sizeDelta = new Vector2(progressBarRect.sizeDelta.x + 3, progressBarRect.sizeDelta.y);
            // If the user has reached the end of the level, fade out progress bar
            if (progressBarRect.sizeDelta.x >= PROGRESS_BAR_FULL_WIDTH && progressBarColor.a > 0)
            {
                progressBarColor.a = progressBarColor.a - 0.01f;
                progressBarRef.color = progressBarColor;
            }
        }
    }

    /**
     * Sets the level to the current level
     * @param level The level to set to
     */
    public void SetLevel(int level)
    {
        this.level = level;
    }

    /**
     * Initializes the level
     */
    public void InitLevel()
    {
        // Initialize the member variables
        numShipsOnScreen = 0;
        levelComplete = false;
        levelOver = false;

        // Reset number of ships
        numShipsDestroyed = 0;

        // Equation to calculate the amount of ships needed to destroy to move on to next level
        numShipsToDestroy = (int)Mathf.Floor(3.3709f * Mathf.Log(level, 2.7183f) + 7.3771f);
        progressAmt = PROGRESS_BAR_FULL_WIDTH / numShipsToDestroy; // Set the amount each ship destroyed should increase the progress bar
        
        // Begins to spawn objects
        SpawnObjects();
    }

    /**
     * Spawns all of the objects for the level
     */
    void SpawnObjects()
    {
        // TODO Add functionality for more ships
        float timeTilNextShip = Random.Range(2f, 4f); // TODO Change with the level
        Invoke("CreateShip", timeTilNextShip);

        float timeTilNextTreasure = Random.Range(5f, 10f);
        Invoke("CreateTreasure", timeTilNextTreasure);

        float timeTilNextRock = Random.Range(1f, 3f);
        Invoke("CreateRock", timeTilNextRock);

        if(level >= 3)
        {
            float timeTilNextShark = Random.Range(6f, 8f);
            Invoke("CreateShark", timeTilNextShark);
        }
    }

    /**
     * Creates a ship
     */
    public void CreateShip() // TODO Figure out what spawns when
    {
        if (!levelComplete && !levelOver)
        {
            if (numShipsOnScreen <= 5) // The number of ships on the screen should never go more than 5 TODO Possibly get rid of this?
            {
                float shipChoice = Random.value; // Determines which size rock to create
                GameObject s = null;
                if (level >= 2 && shipChoice > 0.8f && numBrigantinesOnScreen < 2) // Chances of getting a sloop are higher than a brigantine
                {
                    numBrigantinesOnScreen++;
                    s = Instantiate(brigantine, new Vector3(OFFSCREEN_X, Random.Range(-OFFSCREEN_Y + 1f, OFFSCREEN_Y - 1f)), Quaternion.identity);
                }
                else
                    s = Instantiate(sloop, new Vector3(OFFSCREEN_X, Random.Range(-OFFSCREEN_Y + 0.5f, OFFSCREEN_Y - 0.5f)), Quaternion.identity);

                s.transform.SetParent(ships);
                numShipsOnScreen++; // Increase the amount of ships on the screen

                float timeTilNextShip = Random.Range(3f, 5f); // TODO Change with the level
                Invoke("CreateShip", timeTilNextShip);
            }
            else // Try again later
            {
                float timeTilNextShip = Random.Range(2f, 4f); // TODO Change with the level
                Invoke("CreateShip", timeTilNextShip);
            }
        }
    }

    /**
     * Creates treasure
     */
    public void CreateTreasure()
    {
        if (!levelComplete && !levelOver)
        {
            GameObject t = Instantiate(treasureChest, new Vector3(OFFSCREEN_X, Random.Range(-OFFSCREEN_Y + 0.5f, OFFSCREEN_Y - 0.5f)), Quaternion.identity);
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
        if (!levelComplete && !levelOver)
        {
            float rockChoice = Random.value; // Determines which size rock to create
            GameObject r = null;
            if (rockChoice < 0.8) // Chances of getting a small rock are higher than a large rock
                r = Instantiate(rockSmall, new Vector3(OFFSCREEN_X, Random.Range(-OFFSCREEN_Y + 0.5f, OFFSCREEN_Y - 0.5f)), Quaternion.identity);
            else
                r = Instantiate(rockLarge, new Vector3(OFFSCREEN_X, Random.Range(-OFFSCREEN_Y, OFFSCREEN_Y)), Quaternion.identity);
            r.transform.SetParent(obstacles);

            float timeTilNextRock = Random.Range(1f, 2f);
            Invoke("CreateRock", timeTilNextRock);
        }
    }

    /**
     * Creates a shark
     */
    public void CreateShark()
    {
        if (!levelComplete && !levelOver)
        {
            GameObject t = Instantiate(shark, new Vector3(OFFSCREEN_X, Random.Range(-OFFSCREEN_Y + 2f, OFFSCREEN_Y - 2f)), Quaternion.identity);
            t.transform.SetParent(obstacles);

            float timeTilNextShark = Random.Range(8f, 12f);
            Invoke("CreateShark", timeTilNextShark);
        }
    }

    /**
     * Shows the player how many points they just got
     */
    public void ShowScoreIncrease(int score, Vector3 pos)
    {
        GameObject scoreIncrease = Instantiate(scoreIncreaseText, GameObject.FindObjectOfType<Canvas>().transform);
        (scoreIncrease.GetComponent<ScoreIncreaseTextController>() as ScoreIncreaseTextController).SetParams(score, pos);
    }

    /**
     * Ends the game
     */
    public void EndGame()
    {
        if (!levelOver) // So we can call the same function with different functionaliy each time
        {
            levelOver = true;
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Ship"))
                (obj.GetComponent<Ship>() as Ship).StopMoving();
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Cannonball"))
                (obj.GetComponent<Cannonball>() as Cannonball).StopMoving();
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Treasure"))
                (obj.GetComponent<SimpleObject>() as SimpleObject).StopMoving();
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Rock"))
                (obj.GetComponent<SimpleObject>() as SimpleObject).StopMoving();
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Shark"))
                (obj.GetComponent<Shark>() as Shark).StopMoving();

            SoundManager.instance.SetBackgroundMusic(null); // Turn off music

            Invoke("EndGame", 2f); // Delete everything 2 seconds later (this is so the player sees how they died)
        }
        else
        {
            GameManager.instance.GameOver();
        }
    }

    /**
     * Ends the level
     */
    void EndLevel()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Ship"))
            (obj.GetComponent<Ship>() as Ship).MoveOffScreen();

        Invoke("ShowStats", 12f);
    }

    /**
     * Calls GameManager's show level stats function
     */
    void ShowStats()
    {
        if (numShipsOnScreen != 0) // Make sure we don't end the level while there is a ship onscreen
            Invoke("ShowStats", 1f);
        else
        {
            // Show the end level score increase
            ShowScoreIncrease(200 * level, new Vector3(-8, 0)); // We know the player will be at (-8, 0)

            GameManager.instance.ShowLevelStats();
        }
    }

    /**
     * Removes a ship from numShipsOnScreen & adds to the amt destroyed
     * @param destroyed Whether or not the player destroyed the ship
     */
    public void RemoveShip(bool destroyed, string shipType)
    {
        numShipsOnScreen--;
        switch(shipType) // Make sure not too many higher level ships are onscreen at once
        {
            case "Brigantine(Clone)":
                numBrigantinesOnScreen--;
                break;
            default:
                break;
        }

        if (destroyed) // If the ship was destroyed...
        {
            progressBarWidth += progressAmt; // Increase desired progress bar width
            if (progressBarWidth > PROGRESS_BAR_FULL_WIDTH) // Make sure we don't go overboard
                progressBarWidth = PROGRESS_BAR_FULL_WIDTH; // Also stops progress bar from going over if player destroys ships after level is over

            numShipsDestroyed++;
            if (numShipsDestroyed == numShipsToDestroy) // If the game should end
            {
                if (progressBarWidth != PROGRESS_BAR_FULL_WIDTH) // Fill up the progress bar (if it isn't already)
                    progressBarWidth = PROGRESS_BAR_FULL_WIDTH;
                levelComplete = true;
                Invoke("EndLevel", 0.25f); // Wait for the last destroyed ship to turn to a broken ship
            }
        }
    }

    /**
     * Destroy everything on delete
     */
    void OnDestroy()
    {
        // Make sure there are no new spawns
        CancelInvoke("CreateShip");
        CancelInvoke("CreateRock");
        CancelInvoke("CreateShark");
        CancelInvoke("CreateTreasure");

        if (ships != null)
            Destroy(ships.gameObject); // Destroys all ships
        if (cannonballs != null)
            Destroy(cannonballs.gameObject); // Destroys all cannonballs
        if (obstacles != null)
            Destroy(obstacles.gameObject); // Destroys all obstacles
        if (treasure != null)
            Destroy(treasure.gameObject); // Destroys all treasure
        if(GameObject.FindObjectOfType<Image>().gameObject != null)
            Destroy(GameObject.FindObjectOfType<Image>().gameObject); // Destroy the progress bar
    }
}

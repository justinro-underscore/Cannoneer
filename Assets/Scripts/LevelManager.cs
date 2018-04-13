using System.Collections;
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
    
    public GameObject cannonball; // Prefab for Cannonball
    public GameObject sloop; // Prefab for Sloop
    public GameObject broken; // Prefab for Broken Ship
    public GameObject rockSmall; // Prefab for Small Rock
    public GameObject rockLarge; // Prefab for Large Rock
    public GameObject treasureChest; // Prefab for Treasure Chest
    
    private const int OFFSCREEN_X = 14; // The number that indicates when something goes offscreen in x
    private const int OFFSCREEN_Y = 5; // The number that indicates when something goes offscreen in y

    private int level; // Current level

    private int numShipsToDestroy; // The amount of ships to destroy to end the level
    private int numShipsOnScreen; // The number of ships on screen at one time
    private bool levelOver; // Whether or not the level has completed

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
        // Holds all the rocks created
        rocks = new GameObject("Rocks").transform;
        // Holds all the treasure chests created
        treasure = new GameObject("Treasure").transform;

        //Call the InitGame function to initialize the level
        // The wait is to have time to set the level
        Invoke("InitLevel", 0.05f);
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
        levelOver = false;

        // Reset number of ships
        numShipsDestroyed = 0;
        
        // Equation to calculate the amount of ships needed to destroy to move on to next level
        numShipsToDestroy = (int)Mathf.Floor(3.3709f * Mathf.Log(level, 2.7183f) + 7.3771f);
        
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
        Invoke("CreateSloop", timeTilNextShip);

        float timeTilNextTreasure = Random.Range(5f, 10f);
        Invoke("CreateTreasure", timeTilNextTreasure);

        float timeTilNextRock = Random.Range(1f, 3f);
        Invoke("CreateRock", timeTilNextRock);
    }

    /**
     * Creates a Sloop ship
     */
    public void CreateSloop()
    {
        if (!levelOver)
        {
            if (numShipsOnScreen <= 5) // The number of ships on the screen should never go more than 5 TODO Possibly get rid of this?
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

            float timeTilNextRock = Random.Range(1f, 2f);
            Invoke("CreateRock", timeTilNextRock);
        }
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
            GameManager.instance.ShowLevelStats();
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
                Invoke("EndLevel", 0.25f); // Wait for the last destroyed ship to turn to a broken ship
            }
        }
    }

    /**
     * Destroy everything on delete
     */
    void OnDestroy()
    {
        CancelInvoke(); // Make sure there is no new invokes
        if(ships != null)
            Destroy(ships.gameObject); // Destroys all ships
        if (cannonballs != null)
            Destroy(cannonballs.gameObject); // Destroys all cannonballs
        if (rocks != null)
            Destroy(rocks.gameObject); // Destroys all rocks
        if (treasure != null)
            Destroy(treasure.gameObject); // Destroys all treasure
    }
}

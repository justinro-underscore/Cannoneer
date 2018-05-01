using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : Character
{
    private bool canShootUp; // Whether or not the player can shoot upwards
    private int upIndex; // The current index of the up cannon's sprite
    private bool canShootDown; // Whether or not the player can shoot downwards
    private int downIndex; // The current index of the down cannon's sprite
    public Image cannonUp; // The up cannon UI
    public Image cannonDown; // The down cannon UI
    private Sprite[] cannonGraphics; // The sprites that the cannon UIs will change to

    public Image life1; // The first life lost
    public Image life2; // The second life lost
    private int livesLeft = 0; // The amount of lives the player has left

    private bool isDead; // Whether or not the player has lost

    private bool debugMode; // If true, you are in debug mode

    /**
     * Restarts all values to make sure it is ready for playing
     */
    public void Restart()
    {
        base.Instantiate(); // Get the Rigidbody
        MovePlayer(new Vector2(-8, 0)); // Move to original position
        canShootUp = true;
        canShootDown = true;
        speed = 4; // The speed at which the ship moves
        isDead = false;
        debugMode = false;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f); // Make player visible

        cannonGraphics = Resources.LoadAll<Sprite>("Sprites/CannonSpriteSheet"); // Gets all cannon sprites
        upIndex = 0;
        downIndex = 15;
        cannonUp.sprite = cannonGraphics[14];
        cannonDown.sprite = cannonGraphics[29];
        if(livesLeft == 0) // New game
        {
            cannonUp.color = new Color(1, 1, 1, 1);
            cannonDown.color = new Color(1, 1, 1, 1);

            life1.color = new Color(1, 1, 1, 1);
            life2.color = new Color(1, 1, 1, 1);
            livesLeft = 3;
        }
        InvokeRepeating("SetCannonImage", 0f, 0.2f);
    }
	
    /**
     * Move & get input
     */
	void FixedUpdate ()
	{
        if(gameObject.activeSelf && !isDead) // If player is dead, stop moving
        {
            Move();

            // Shoot the cannons
		    if (Input.GetKeyDown (KeyCode.JoystickButton1))
			    ShootCannonBall (true);
		    else if (Input.GetKeyDown (KeyCode.JoystickButton0))
			    ShootCannonBall (false);
        }
	}

    /**
     * Moves the player
     */
    void Move()
    {
        Vector2 move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        rb2d.velocity = move * speed;
    }

    /**
     * Shoots the cannonball either upwards or downwards
     * @param dirUp whether to shoot up or down
     */
    void ShootCannonBall(bool dirUp)
	{
		if (debugMode || (dirUp ? canShootUp : canShootDown)) // Check to see if it should shoot the cannon
		{
            SoundManager.instance.PlaySingle("cannonPlayerFire"); // Sound the cannons!

            // Create the cannonball
            CreateCannonball(true, dirUp);

            // Reset the cannon reload
            if (dirUp)
                canShootUp = false;
            else
                canShootDown = false;
		}
    }

    /**
     * Creates a cannonball object
     * @param shotByPlayer whether or not the player fired the cannonball
     * @param dirUp which direction the cannonball should go (true for up, false for down)
     */
    void CreateCannonball(bool shotByPlayer, bool dirUp)
    {
        // Creates the cannonball
        GameObject ball = Instantiate(LevelManager.instance.cannonball, transform.position, Quaternion.identity);
        (ball.GetComponent<Cannonball>() as Cannonball).SetParams(shotByPlayer, dirUp, rb2d.velocity.x); // Sets the parameters
        ball.transform.SetParent(LevelManager.instance.cannonballs); // Organizes the heirarchy
    }

    /**
     * Shows the player how long until they can shoot again
     */
    void SetCannonImage()
    {
        if(!canShootUp)
        {
            cannonUp.sprite = cannonGraphics[upIndex++];
            if(upIndex == 15) // 15 is the highest index
            {
                upIndex = 0; // Reset
                canShootUp = true;
            }
        }
        if(!canShootDown)
        {
            cannonDown.sprite = cannonGraphics[downIndex++];
            if (downIndex == 30) // 30 is the highest index
            {
                downIndex = 15; // Reset
                canShootDown = true;
            }
        }
    }

    /**
     * If player hit cannonball (shot by ship) or hits ship or hits rock  or hits shark game over
     * If player touches treasure, increase score
     * @param other The other object it collides with
     */
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ship") ||
            other.gameObject.CompareTag("Rock") ||
            other.gameObject.CompareTag("Shark") ||
            other.gameObject.CompareTag("Explosion") ||
            (other.gameObject.CompareTag("Cannonball") && !(other.GetComponent<Cannonball>() as Cannonball).getShotByPlayer()))
        {
            if(!debugMode)
                GameOver();
        }
        else if(other.gameObject.CompareTag("Treasure"))
        {
            GameManager.instance.IncreaseScore(100, "treasure");
            LevelManager.instance.ShowScoreIncrease(100, transform.position); // Show how many points the player got

            Destroy(other.gameObject);
        }
    }

    /**
     * GAME OVER (if player dies)
     */
    void GameOver()
    {
        ToggleIsDead(); // Die
        SoundManager.instance.StopSounds();
        SoundManager.instance.PlaySingle("explosionPlayer");
        CancelInvoke("SetCannonImage");
        livesLeft--;

        LevelManager.instance.ClearLevel(livesLeft == 0);
    }

    /**
     * Removes a life from the UI
     */
    public void RemoveLife()
    {
        if (livesLeft <= 2)
            life1.color = new Color(1, 1, 1, 0);
        if (livesLeft <= 1)
            life2.color = new Color(1, 1, 1, 0);
    }

    /**
     * Removes the cannon UI
     */
    public void CannonHide()
    {
        cannonUp.color = new Color(1, 1, 1, 0);
        cannonDown.color = new Color(1, 1, 1, 0);
    }

    /**
     * Toggles if the player is dead or not
     */
    public void ToggleIsDead()
    {
        isDead = !isDead;
        rb2d.velocity = new Vector3(0, 0); // Stop moving
    }

    /**
     * Moves the player object to the specified position vector
     */
    public void MovePlayer(Vector2 position)
    {
        rb2d.position = position;
    }

    /**
     * Gets the player's debug mode status
     * @return debugMode Whether or not the player is debugging
     */
    public bool GetDebugMode()
    {
        return debugMode;
    }

    /**
     * Changes the player's debug mode status
     */
    public void ToggleDebugMode()
    {
        debugMode = !debugMode;
        if (!debugMode) // If the player deactivates the mode, game over
        {
            livesLeft = 1;
            RemoveLife();
            GameOver();
        }
    }
}

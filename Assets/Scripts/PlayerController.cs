using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : Character
{
    protected int timeSinceLastCannonUp; // The time since the last cannon was shot up
    protected int timeSinceLastCannonDown; // The time since the last cannon was shot down
    private const int TIME_TIL_FIRE = 200; // The amt of time to wait between cannon fires

    // TODO: Change to a graphic
    public Text cannonText; // The text showing how much time is left for the cannon

    private bool isDead = false; // Whether or not the player has lost

    /**
     * Initializes the player
     */
	void Start ()
	{
        base.Instantiate(); // Get the Rigidbody
        timeSinceLastCannonUp = 0;
        timeSinceLastCannonDown = 0;
        speed = 4; // The speed at which the ship moves
		SetCannonText (); // TODO Change to a graphic
	}
	
    /**
     * Move & get input
     */
	void FixedUpdate ()
	{
        if(!isDead) // If player is dead, stop moving
        {
            Move();

            // Decrease time to shoot
            if (timeSinceLastCannonUp != 0)
			    timeSinceLastCannonUp--;
		    if(timeSinceLastCannonDown != 0)
			    timeSinceLastCannonDown--;
		    SetCannonText ();

            // Shoot the cannons
		    if (Input.GetKeyDown ("o"))
			    ShootCannonBall (true);
		    else if (Input.GetKeyDown ("l"))
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
		if (dirUp ? timeSinceLastCannonUp == 0 : timeSinceLastCannonDown == 0) // Check to see if it should shoot the cannon
		{
            // Create the cannonball
            CreateCannonball(true, dirUp);

            // Reset the cannon reload
            if (dirUp)
                timeSinceLastCannonUp = TIME_TIL_FIRE;
            else
                timeSinceLastCannonDown = TIME_TIL_FIRE;
		}
	}

    /**
     * TODO Switch to graphic
     * Shows the player how long until they can shoot again
     */
	void SetCannonText()
	{
		cannonText.text = string.Format ("Up: {0:000} Down: {1:000}", timeSinceLastCannonUp, timeSinceLastCannonDown);
	}

    /**
     * If player hit cannonball (shot by ship) or hits ship or hits rock game over
     * If player touches treasure, increase score
     * @param other The other object it collides with
     */
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ship") ||
            other.gameObject.CompareTag("Rock") ||
            (other.gameObject.CompareTag("Cannonball") && !(other.GetComponent<Cannonball>() as Cannonball).getShotByPlayer()))
        {
            GameOver();
        }
        else if(other.gameObject.CompareTag("Treasure"))
        {
            LevelManager.instance.IncreaseScore(100);
            Destroy(other.gameObject);
        }
    }

    /**
     * GAME OVER (if player dies)
     */
    void GameOver()
    {
        isDead = true;
        rb2d.velocity = new Vector3(0, 0); // Stop moving
        LevelManager.instance.EndGame(); // End the game
    }
}

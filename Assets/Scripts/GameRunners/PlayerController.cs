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

    private bool isDead; // Whether or not the player has lost

    private bool godMode; // If true, you cannot die and can shoot infinitely

    /**
     * Restarts all values to make sure it is ready for playing
     */
    public void Restart()
    {
        base.Instantiate(); // Get the Rigidbody
        MovePlayer(new Vector2(-8, 0)); // Move to original position
        timeSinceLastCannonUp = 0;
        timeSinceLastCannonDown = 0;
        speed = 4; // The speed at which the ship moves
        isDead = false;
        godMode = false;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f); // Make player visible
		SetCannonText (); // TODO Change to a graphic
    }
	
    /**
     * Move & get input
     */
	void FixedUpdate ()
	{
        if(gameObject.activeSelf && !isDead) // If player is dead, stop moving
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
        Vector2 move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")); // TODO Make this more reactive
        rb2d.velocity = move * speed;
    }

    /**
     * Shoots the cannonball either upwards or downwards
     * @param dirUp whether to shoot up or down
     */
    void ShootCannonBall(bool dirUp)
	{
		if (godMode || (dirUp ? timeSinceLastCannonUp == 0 : timeSinceLastCannonDown == 0)) // Check to see if it should shoot the cannon
		{
            SoundManager.instance.PlaySingle("cannonFire"); // Sound the cannons!

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
     * If player hit cannonball (shot by ship) or hits ship or hits rock  or hits shark game over
     * If player touches treasure, increase score
     * @param other The other object it collides with
     */
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ship") ||
            other.gameObject.CompareTag("Rock") ||
            other.gameObject.CompareTag("Shark") ||
            (other.gameObject.CompareTag("Cannonball") && !(other.GetComponent<Cannonball>() as Cannonball).getShotByPlayer()))
        {
            if(!godMode)
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
        SoundManager.instance.PlaySingle("explosionPlayer");
        cannonText.text = "";
        LevelManager.instance.EndGame(); // End the game
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
}

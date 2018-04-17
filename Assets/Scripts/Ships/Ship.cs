using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Ship : Character
{
    public Text scoreIncreaseText;

    protected Transform target; // Ships know where Player is
    protected int score; // Amt of points Player gets for destroying the ship
    protected bool canShoot; // If the ship can shoot now
    protected bool canSink; // Whether or not the ship can sink
    protected bool goOffScreen; // When the level ends, the ships should just go offscreen
    protected int livesLeft; // The amount of hits the ship can take before being destroyed

    /**
     * Instantiates the ship
     */
    protected new void Instantiate ()
    {
        base.Instantiate(); // Calls Character instantiation

        //Find the Player GameObject using it's tag and store a reference to its transform component.
        target = GameObject.FindGameObjectWithTag("Player").transform;
        canShoot = true;
        canSink = true;
        goOffScreen = false;
    }

    /**
     * Check to see if the ship goes offscreen
     */
    protected void CheckIfOffScreen()
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewPos.x < -0.2f) // If it goes too far left, destroy it
        {
            if (!(name.Equals("BrokenShip(Clone)")))
                LevelManager.instance.RemoveShip(false, name);
            Destroy(gameObject); // Destroy the object
        }
    }

    /**
     * If ship hit cannonball (shot by player) it sinks
     * @param other The other object it collides with
     */
    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (canSink && other.gameObject.CompareTag("Cannonball") && (other.GetComponent<Cannonball>() as Cannonball).getShotByPlayer())
        {
            Destroy(other.gameObject, 0.25f); // Destroy the cannonball
            livesLeft--;
            if (livesLeft == 0) // If the ship is destroyed
            {
                DestroyShip(); // Destroy the ship
            }
            else
            {
                canSink = false;
                Invoke("ToggleCanSink", 0.25f);
                ChangeSprite();
            }
        }
    }

    /**
     * Changes the sprite of the ship (different for each ship)
     */
    protected abstract void ChangeSprite();

    /**
     * Destroys the ship
     */
    void DestroyShip()
    {
        if (canSink)
        {
            // TODO Show explosion animation
            SoundManager.instance.PlaySingle("explosionEnemy");
            if (!(name.Equals("BrokenShip(Clone)"))) // If the ship isn't a broken ship...
            {
                GameManager.instance.IncreaseScore(score, "ship"); // Increase the score
                // Turn it into a ship
                GameObject broken = Instantiate(LevelManager.instance.broken, transform.position, Quaternion.identity);
                broken.transform.SetParent(LevelManager.instance.ships);
                LevelManager.instance.RemoveShip(true, name);
            }
            else
                GameManager.instance.IncreaseScore(score, "obstacle"); // Increase the score as an obstacle
            LevelManager.instance.ShowScoreIncrease(score, transform.position); // Show how many points the player got

            Destroy(gameObject); // Destroy this ship
        }
    }

    /**
     * Shoots the cannonball in a direction
     * @param dirUp If it should shoot upwards or downwards
     * @param targetPlayer Whether or not the ship should target the player with its shot
     */
    protected void ShootCannonBall(bool dirUp, bool targetPlayer)
    {
        if (canShoot) // Only shoot if reload time is done
        {
            // Create the cannonball
            GameObject ball = Instantiate(LevelManager.instance.cannonball, transform.position, Quaternion.identity);
            float xVelocity = 0f;
            if (targetPlayer)
                xVelocity = (target.position.x - transform.position.x); // Aim at player
            else
                xVelocity = rb2d.velocity.x;
            (ball.GetComponent<Cannonball>() as Cannonball).SetParams(false, dirUp, xVelocity); // Set the parameters
            ball.transform.SetParent(LevelManager.instance.cannonballs);
            canShoot = false; // Reload

            Invoke("ToggleCanShoot", 2); // Reload for 2 seconds
        }
    }

    /**
     * Toggles the canShoot variable
     */
    protected void ToggleCanShoot()
    {
        canShoot = !canShoot;
    }

    /**
     * Toggles the canSink variable
     */
    protected void ToggleCanSink()
    {
        canSink = !canSink;
    }

    /**
     * What happens when the level is finished
     */
    public void MoveOffScreen()
    {
        rb2d.velocity = new Vector2(-2f, 0); // Go offscreen at a constant velocity
        goOffScreen = true;
    }
    
    /**
     * Stops the object from moving
     */
    public void StopMoving()
    {
        CancelInvoke("ToggleCanShoot"); // Stops them from shooting
        rb2d.velocity = new Vector3(0, 0);
    }
}

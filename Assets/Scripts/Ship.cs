using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : Character
{
    protected Transform target; // Ships know where Player is
    protected int score; // Amt of points Player gets for destroying the ship
    protected bool canShoot; // If the ship can shoot now
    protected bool canSink; // Whether or not the ship can sink
    protected bool goOffScreen; // When the level ends, the ships should just go offscreen

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
            if(!(this is BrokenShip))
                LevelManager.instance.RemoveShip(false);
            Destroy(gameObject); // Destroy the object
        }
    }

    /**
     * If ship hit cannonball (shot by player) it sinks
     * @param other The other object it collides with
     */
    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Cannonball") && (other.GetComponent<Cannonball>() as Cannonball).getShotByPlayer())
        {
            Destroy(other.gameObject, 0.25f); // Destroy the cannonball
            DestroyShip(); // Destroy the ship
        }
    }

    /**
     * Destroys the ship
     */
    void DestroyShip()
    {
        if (canSink)
        {
            // TODO Show explosion animation
            if (!(this is BrokenShip)) // If the ship isn't a broken ship...
            {
                GameManager.instance.IncreaseScore(score, "ship"); // Increase the score
                // Turn it into a ship
                GameObject broken = Instantiate(LevelManager.instance.broken, transform.position, Quaternion.identity);
                broken.transform.SetParent(LevelManager.instance.ships);
                LevelManager.instance.RemoveShip(true);
            }
            else
                GameManager.instance.IncreaseScore(score, "obstacle"); // Increase the score as an obstacle
            Destroy(gameObject); // Destroy this ship
        }
    }

    /**
     * Shoots the cannonball in a direction
     * @param dirUp If it should shoot upwards or downwards
     */
    protected void ShootCannonBall(bool dirUp)
    {
        if (canShoot) // Only shoot if reload time is done
        {
            // Create the cannonball
            GameObject ball = Instantiate(LevelManager.instance.cannonball, transform.position, Quaternion.identity);
            (ball.GetComponent<Cannonball>() as Cannonball).SetParams(false, dirUp, rb2d.velocity.x); // Set the parameters
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
        CancelInvoke(); // Stops them from shooting
        rb2d.velocity = new Vector3(0, 0);
    }
}

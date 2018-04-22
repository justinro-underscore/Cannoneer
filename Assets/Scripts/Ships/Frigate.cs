using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frigate : Ship
{
    public Sprite broken;
    private bool shotOnce; // Whether or not the ship has fired its first shot yet (so it can double fire)

    /**
     * Initializes the ship
     */
    void Start()
    {
        base.Instantiate();
        score = 400; // Score if destroyed
        rb2d.velocity = new Vector2(-2.5f, 0.1f); // Set move function, Brigantine initially goes at constant velocity
        livesLeft = 2;
        shotOnce = false;

        Invoke("TurnHorizontal", Random.Range(3f, 5f));
    }

    /**
     * Check to see if it should shoot
     */
    void FixedUpdate()
    {
        CheckIfOffScreen();
        if(ShootCannonBall(target.position.y > transform.position.y, true)) // If the cannon is fired
        {
            if (shotOnce) // If it's the second time being fired...
                shotOnce = false;
            else
            {
                shotOnce = true;
                CancelInvoke("ToggleCanShoot");
                Invoke("ToggleCanShoot", 0.5f); // Shoot again 0.5 seconds later
            }
        }
    }

    /**
     * Changes the sprite of the ship
     */
    protected override void ChangeSprite()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = broken;
    }

    #pragma warning disable CS0108 // I got tired of seeing this dumb warning message IT WON'T BREAK
    /**
     * If ship hit cannonball (shot by player) it sinks
     * @param other The other object it collides with
     */
    protected void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        if (!(other.CompareTag("Cannonball") || // Make sure Brigantine doesn't go through another object
            other.name.Equals("Background") ||
            other.CompareTag("Explosion") ||
            other.CompareTag("Treasure")))
        {
            CancelInvoke("TurnHorizontal");
            TurnHorizontal();
        }
        if(other.name.Equals("Background")) // If it hits the edges of the screen it will turn vertically
        {
            CancelInvoke("TurnVertical");
            TurnVertical();
        }
    }

    /**
     * Changes direction horizontally
     */
    void TurnHorizontal()
    {
        if (rb2d.velocity.x != 0) // If the ship has stopped moving
        {
            rb2d.velocity = new Vector3((rb2d.velocity.x < 0 ? 1f : -2.5f), rb2d.velocity.y); // Go backwards slow or go forwards fast (toggle)

            float timeTilNextTurn;
            if (rb2d.velocity.x < 0)
                timeTilNextTurn = Random.Range(3f, 5f); // Go farther back than forward
            else
                timeTilNextTurn = Random.Range(1f, 2f);
            Invoke("TurnHorizontal", timeTilNextTurn);
        }
    }

    /**
     * Changes direction vertically
     */
    void TurnVertical()
    {
        if (rb2d.velocity.y != 0) // If the ship has stopped moving
        {
            rb2d.velocity = new Vector3(rb2d.velocity.x, (rb2d.velocity.y < 0 ? 0.5f : -0.5f));
            
            Invoke("TurnVertical", 1f);
        }
    }
}

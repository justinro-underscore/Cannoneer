using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brigantine : Ship
{
    /**
     * Initializes the ship
     */
    void Start()
    {
        base.Instantiate();
        score = 250; // Score if destroyed
        rb2d.velocity = new Vector2(-2.5f, 0); // Set move function, Brigantine initially goes at constant velocity

        Invoke("Turn", Random.Range(3f, 5f));
    }

    /**
     * Check to see if it should shoot
     */
    void FixedUpdate()
    {
        CheckIfOffScreen();
        CheckForShoot();
    }

    /**
     * Check to see where to shoot
     */
    void CheckForShoot()
    {
        ShootCannonBall(target.position.y > transform.position.y, true);
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
            other.CompareTag("Treasure")))
        {
            CancelInvoke("Turn");
            Turn();
        }
    }

    /**
     * Changes direction
     */
    void Turn()
    {
        rb2d.velocity = new Vector3((rb2d.velocity.x < 0 ? 1f : -2.5f), 0); // Go backwards slow or go forwards fast (toggle)

        float timeTilNextTurn;
        if (rb2d.velocity.x < 0)
            timeTilNextTurn = Random.Range(3f, 5f); // Go farther back than forward
        else
            timeTilNextTurn = Random.Range(1f, 2f);
        Invoke("Turn", timeTilNextTurn);
    }
}

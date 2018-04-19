using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sloop : Ship
{
    /**
     * Initializes the ship
     */
    void Start()
    {
        base.Instantiate();
        score = 150; // Score if destroyed
        livesLeft = 1;
        rb2d.velocity = new Vector2(-2f, 0); // Set move function, Sloop goes at constant velocity
    }

    /**
     * Check to see if it should shoot
     */
    void FixedUpdate()
    {
        CheckIfOffScreen();
        ShootCannonBall(target.position.y > transform.position.y, false);
    }

    /**
     * Changes the sprite of the ship
     */
    protected override void ChangeSprite()
    {
        // Do nothing
    }
}

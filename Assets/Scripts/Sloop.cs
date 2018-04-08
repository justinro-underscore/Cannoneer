using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sloop : Ship
{
    /**
     * Initializes the ship
     */
	void Start ()
    {
        base.Instantiate();
        score = 150; // Score if destroyed
        rb2d.velocity = new Vector2(-2f, 0); // Set move function, Sloop goes at constant velocity
    }

    /**
     * Check to see if it should shoot
     */
	void FixedUpdate()
    {
        CheckIfOffScreen();
        if (!goOffScreen)
        {
            CheckForShoot();
        }
    }

    /**
     * Check to see where to shoot
     */
    void CheckForShoot()
    {
        ShootCannonBall(target.position.y > transform.position.y);
    }
}

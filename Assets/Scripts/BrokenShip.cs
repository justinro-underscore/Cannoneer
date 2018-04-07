using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenShip : Ship
{
    /**
     * Initializes the ship
     */
    void Start()
    {
        base.Instantiate();
        rb2d.velocity = new Vector2(-2f, 0); // Set move function, Sloop goes at constant velocity
    }

    /**
     * Check to see if it has gone offscreen
     */
    void FixedUpdate()
    {
        CheckIfOffScreen();
    }
}

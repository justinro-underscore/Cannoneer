﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : Character
{
    protected Transform target; // Ships know where Player is
    protected int score; // Amt of points Player gets for destroying the ship
    protected bool canShoot; // If the ship can shoot now
    protected bool canSink; // Whether or not the ship can sink

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
    }

    /**
     * Check to see if the ship goes offscreen
     */
    protected void CheckIfOffScreen()
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewPos.x < 0) // If it goes too far left, destroy it
        {
            Destroy(gameObject, 1f); // Wait a second before destroying so that it doesn't disappear as soon as it touches the edge
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
            LevelManager.instance.IncreaseScore(score); // Increase the score
            if (!(this is BrokenShip)) // If the ship isn't a broken ship...
            {
                // Turn it into a ship
                GameObject broken = Instantiate(LevelManager.instance.broken, transform.position, Quaternion.identity);
                broken.transform.SetParent(LevelManager.instance.ships);
            }
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
}

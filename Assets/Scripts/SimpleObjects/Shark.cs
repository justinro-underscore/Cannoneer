using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shark : SimpleObject
{
    #pragma warning disable CS0108 // I got tired of seeing this dumb warning message IT'S FINE IT WORKS
    /**
     * Moves in a cosine wave
     */
    void Update ()
    {
        base.Update();
        if(rb2d.velocity.x != 0) // If the object has not stopped moving
            rb2d.velocity = new Vector3(-4, 6 * Mathf.Cos(Time.time * 3)); // Move in a cosine wave pattern
    }

    /**
     * If shark gets hit by a cannonball fired by player, it dies
     * @param other The other object it collides with
     */
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Cannonball") && (other.GetComponent<Cannonball>() as Cannonball).getShotByPlayer())
        {
            GameManager.instance.IncreaseScore(300, "obstacle"); // Increase the score as an obstacle
            LevelManager.instance.ShowScoreIncrease(300, transform.position); // Show how many points the player got
            Destroy(gameObject);
        }
    }
}

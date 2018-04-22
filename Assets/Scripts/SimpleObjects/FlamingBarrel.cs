using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamingBarrel : SimpleObject
{
    private Transform target; // Knows where Player is

    /**
     * Gets the player's transform/position
     */
    void Start()
    {
        base.Init();
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

#pragma warning disable CS0108 // I got tired of seeing this dumb warning message IT'S FINE IT WORKS
    /**
     * Moves in a cosine wave
     */
    void Update()
    {
        base.Update();
        if (rb2d.velocity.x != 0) // If the object has not stopped moving
            transform.Rotate(new Vector3(0, 0, 1) * Mathf.Cos(Time.time * 2f) * 0.25f);
        CheckForExplosion();
    }

    /**
     * Check if it is close to the player
     */
    void CheckForExplosion()
    {
        float distance = Mathf.Sqrt(Mathf.Pow(transform.position.x - target.position.x, 2f) + Mathf.Pow(transform.position.y - target.position.y, 2f));
        if (distance < 3f)
            TriggerExplosion();
    }

    /**
     * Blows up the flaming barrel
     */
    void TriggerExplosion()
    {
        LevelManager.instance.Explode(2f, transform.position, rb2d.velocity);
        Destroy(gameObject);
    }

    /**
     * If shark gets hit by a cannonball fired by player, it dies
     * @param other The other object it collides with
     */
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Cannonball") && (other.GetComponent<Cannonball>() as Cannonball).getShotByPlayer())
        {
            GameManager.instance.IncreaseScore(100, "obstacle"); // Increase the score as an obstacle
            LevelManager.instance.ShowScoreIncrease(100, transform.position); // Show how many points the player got
            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : MonoBehaviour
{
    private bool shotByPlayer; // Whether or not the player fired the cannonball (so the player ship won't be destroyed when player fires)
    private bool up; // Direction that the cannonball goes
    private float xSpeed; // Speed of the cannonball in x direction (y direction is constant)
    private Rigidbody2D rb2d; // Reference to the Rigidbody
    
    /**
     * Sets the parameters of the cannonball (because initialize doesn't let you use a constructor)
     * @param shotByPlayer
     * @param up
     * @param xSpeed
     */
    public void SetParams(bool shotByPlayer, bool up, float xSpeed)
    {
        this.shotByPlayer = shotByPlayer;
        this.up = up;
        this.xSpeed = xSpeed * 0.25f; // Decrease by 4 so that they don't go flying forward

        rb2d = GetComponent<Rigidbody2D>();
        rb2d.velocity = new Vector2(this.xSpeed, this.up ? 2f : -2f); // Speed in y-axis is constant
    }

    /**
     * Updates each frame
     */
    void Update ()
    {
        // Check to see if it has gone offscreen
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewPos.y > 1 || viewPos.y < 0 || viewPos.x > 1 || viewPos.x < 0)
        {
            Destroy(gameObject, 0.5f); // Wait half a second before destroying so that it doesn't disappear as soon as it touches the edge
        }

        //Rotate thet transform of the game object this is attached to by 45 degrees, taking into account the time elapsed since last frame.
        transform.Rotate(new Vector3(0, 0, 45) * Time.deltaTime);
    }

    /**
     * @return whether or not the player fired the cannonball
     */
    public bool getShotByPlayer()
    {
        return shotByPlayer;
    }

    /**
     * Stops the object from moving
     */
    public void StopMoving()
    {
        rb2d.velocity = new Vector3(0, 0);
    }
}

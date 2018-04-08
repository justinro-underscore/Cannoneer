using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleObject : MonoBehaviour
{
    private Rigidbody2D rb2d; // So we can manipulate the rigidbody
    private bool checkCollision;

    /**
     * Creates the Object
     */
    void Start ()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.velocity = new Vector2(-2f, 0); // Set move function, object goes at constant velocity
        checkCollision = true;
        Invoke("StopCheckingCollision", 1f);
    }

    /**
     * Check to see if the object goes offscreen
     */
    void Update()
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewPos.x < 0) // If it goes too far left, destroy it
        {
            Destroy(gameObject, 0.5f); // Wait a second before destroying so that it doesn't disappear as soon as it touches the edge
        }
    }

    /**
     * If object spawns on another object, retry
     * @param other The other object it collides with
     */
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject);
        if (checkCollision && (other.gameObject.CompareTag("Ship") ||
            other.gameObject.CompareTag("Rock") ||
            other.gameObject.CompareTag("Treasure")))
        {
            // If object goes into this, destroy and retry
            if (this.gameObject.CompareTag("Rock"))
                LevelManager.instance.CreateRock();
            if (this.gameObject.CompareTag("Treasure"))
                LevelManager.instance.CreateTreasure();

            Destroy(gameObject);
        }
    }

    /**
     * Turn off the checkCollision
     */
    void StopCheckingCollision()
    {
        checkCollision = false;
    }
}

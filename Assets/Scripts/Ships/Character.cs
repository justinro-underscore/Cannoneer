using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    protected float speed; // The speed at which the ship moves
    protected Rigidbody2D rb2d; // So we can manipulate the rigidbody

    /**
     * Creates the Character
     */
    protected void Instantiate()
    {
        //Get and store a reference to the Rigidbody2D component so that we can access it.
        rb2d = GetComponent<Rigidbody2D>();
    }

    /**
     * Creates a cannonball object
     * @param shotByPlayer whether or not the player fired the cannonball
     * @param dirUp which direction the cannonball should go (true for up, false for down)
     */
    protected void CreateCannonball(bool shotByPlayer, bool dirUp)
    {
        // Creates the cannonball
        GameObject ball = Instantiate(LevelManager.instance.cannonball, transform.position, Quaternion.identity);
        (ball.GetComponent<Cannonball>() as Cannonball).SetParams(shotByPlayer, dirUp, rb2d.velocity.x); // Sets the parameters
        ball.transform.SetParent(LevelManager.instance.cannonballs); // Organizes the heirarchy
    }
}
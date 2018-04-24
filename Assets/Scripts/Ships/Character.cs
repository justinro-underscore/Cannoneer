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
}
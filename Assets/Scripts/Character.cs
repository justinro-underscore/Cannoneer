using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    protected float speed; // The speed at which the ship moves
    protected int timeSinceLastCannonUp; // The time since the last cannon was shot up
    protected int timeSinceLastCannonDown; // The time since the last cannon was shot down
    protected int timeToFire; // The amt of time to wait between cannon fires
    protected Rigidbody2D rb2d; // So we can manipulate the rigidbody

    protected void Instantiate()
    {
        timeSinceLastCannonUp = 0;
        timeSinceLastCannonDown = 0;
        timeToFire = 200;

        //Get and store a reference to the Rigidbody2D component so that we can access it.
        rb2d = GetComponent<Rigidbody2D>();
    }
}
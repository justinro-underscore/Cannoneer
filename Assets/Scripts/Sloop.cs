using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sloop : Ship
{
    public Text temp;

	// Use this for initialization
	void Start ()
    {
        base.Instantiate();
        rb2d.velocity = new Vector2(-2f, 0); // Set move function
    }

	void FixedUpdate()
    {
        if (timeSinceLastCannonUp != 0) // We'll just be using timeSinceLastCannonUp so that we don't have to create a new variable
            timeSinceLastCannonUp--;
        CheckForShoot();
    }

    void CheckForShoot()
    {
        temp.text = "" + timeSinceLastCannonUp + "  " + timeToFire;
        ShootCannonBall(true);
    }

    void ShootCannonBall(bool dirUp)
    {
        if (timeSinceLastCannonUp == 0)
        {
            GameObject ball = Instantiate(cannonball, transform.position, Quaternion.identity);
            (ball.GetComponent<Cannonball>() as Cannonball).SetParams(false, dirUp, rb2d.velocity.x);
            ball.transform.SetParent(LevelManager.instance.cannonballs);
            Debug.Log("Keep going");

            timeSinceLastCannonUp = 200;
        }
    }
}

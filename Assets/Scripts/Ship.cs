using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : Character
{
    public GameObject cannonball; // Prefab for cannonball
    protected Transform target; // Ships know where Player is
    protected int score; // Amt of points Player gets for destroying the ship

    // Use this for initialization
    protected new void Instantiate ()
    {
        base.Instantiate();

        //Find the Player GameObject using it's tag and store a reference to its transform component.
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Cannonball"))
        {
            DestroyShip();
        }
    }

    void DestroyShip()
    {
        LevelManager.instance.IncreaseScore(score);
        this.enabled = false; // Change to change sprite
    }

    void ShootCannonBall(bool dirUp)
    {
        if (dirUp ? timeSinceLastCannonUp == 0 : timeSinceLastCannonDown == 0)
        {
            GameObject ball = Instantiate(cannonball, transform.position, Quaternion.identity);
            (ball.GetComponent<Cannonball>() as Cannonball).SetParams(true, dirUp, rb2d.velocity.x);
            ball.transform.SetParent(LevelManager.instance.cannonballs);

            if (dirUp)
                timeSinceLastCannonUp = timeToFire;
            else
                timeSinceLastCannonDown = timeToFire;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : Character
{
    protected Transform target; // Ships know where Player is
    protected int score; // Amt of points Player gets for destroying the ship
    protected bool canShoot;

    // Use this for initialization
    protected new void Instantiate ()
    {
        base.Instantiate();

        //Find the Player GameObject using it's tag and store a reference to its transform component.
        target = GameObject.FindGameObjectWithTag("Player").transform;
        canShoot = true;
    }

    protected void CheckIfOffScreen()
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewPos.x < 0)
        {
            Destroy(gameObject, 1f);
        }
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Cannonball") && (other.GetComponent<Cannonball>() as Cannonball).getShotByPlayer())
        {
            Debug.Log("Hit!");
            Destroy(other.gameObject, 0.25f);
            DestroyShip();
        }
    }

    void DestroyShip()
    {
        LevelManager.instance.IncreaseScore(score);
        GameObject broken = Instantiate(LevelManager.instance.broken, transform.position, Quaternion.identity);
        broken.transform.SetParent(LevelManager.instance.ships);
        Destroy(gameObject);
    }

    protected void ShootCannonBall(bool dirUp)
    {
        if (canShoot)
        {
            GameObject ball = Instantiate(LevelManager.instance.cannonball, transform.position, Quaternion.identity);
            (ball.GetComponent<Cannonball>() as Cannonball).SetParams(false, dirUp, rb2d.velocity.x);
            ball.transform.SetParent(LevelManager.instance.cannonballs);
            canShoot = false;

            Invoke("ToggleCanShoot", 2);
        }
    }

    void ToggleCanShoot()
    {
        canShoot = !canShoot;
    }
}

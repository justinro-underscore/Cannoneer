using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : Character
{
    // TODO: Change to a graphic
	public Text cannonText; // The text showing how much time is left for the cannon

    private bool isDead = false;

	// Use this for initialization
	void Start ()
	{
        base.Instantiate();
		speed = 4;
		SetCannonText ();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
        if(!isDead)
        {
            Move();

            if (timeSinceLastCannonUp != 0)
			    timeSinceLastCannonUp--;
		    if(timeSinceLastCannonDown != 0)
			    timeSinceLastCannonDown--;
		    SetCannonText ();
		    if (Input.GetKeyDown ("o"))
			    ShootCannonBall (true);
		    else if (Input.GetKeyDown ("l"))
			    ShootCannonBall (false);
        }
	}

    void Move()
    {
        Vector2 move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        rb2d.velocity = move * speed;
    }

    void ShootCannonBall(bool dirUp)
	{
		if (dirUp ? timeSinceLastCannonUp == 0 : timeSinceLastCannonDown == 0)
		{
            GameObject ball = Instantiate(LevelManager.instance.cannonball, transform.position, Quaternion.identity);
            (ball.GetComponent<Cannonball>() as Cannonball).SetParams(true, dirUp, rb2d.velocity.x);
            ball.transform.SetParent(LevelManager.instance.cannonballs);

            if (dirUp)
                timeSinceLastCannonUp = timeToFire;
            else
                timeSinceLastCannonDown = timeToFire;
		}
	}

	void SetCannonText()
	{
		cannonText.text = string.Format ("Up: {0:000} Down: {1:000}", timeSinceLastCannonUp, timeSinceLastCannonDown);
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ship") || (other.gameObject.CompareTag("Cannonball") && !(other.GetComponent<Cannonball>() as Cannonball).getShotByPlayer()))
        {
            GameOver();
        }
        else if(other.gameObject.CompareTag("Treasure"))
        {
            LevelManager.instance.IncreaseScore(50);
        }
    }

    void GameOver()
    {
        isDead = true;
        rb2d.velocity = new Vector3(0, 0);
        LevelManager.instance.EndGame();
    }
}

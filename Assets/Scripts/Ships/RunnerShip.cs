using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunnerShip : Ship
{
    public Image skull;
    private Image skullRef;

    /**
     * Initializes the ship
     */
    void Start()
    {
        base.Instantiate();
        score = 300; // Score if destroyed
        livesLeft = 1;
        rb2d.velocity = new Vector2(0, 0); // Set move function, starts nonmoving
        Warning();
    }

    /**
     * 
     */
    void FixedUpdate()
    {
        CheckIfOffScreen();
    }

    /**
     * Changes the sprite of the ship
     */
    protected override void ChangeSprite()
    {
        // Do nothing
    }

    /**
     * Shows the warning sign
     */
    void Warning()
    {
        skullRef = Instantiate(skull, GameObject.FindObjectOfType<Canvas>().transform);
        skullRef.gameObject.transform.position = new Vector3(10, transform.position.y);
        Invoke("Charge", 1f);
    }

    /**
     * Charges the player
     */
    void Charge()
    {
        if(skullRef != null)
        {
            Destroy(skullRef.gameObject); // Get rid of the image
            Invoke("Charge", 0.25f);
        }
        else
        {
            if(LevelManager.instance.GetLevel() <= 7) // If level is less than or equal to 7, just go straight
                rb2d.velocity = new Vector2(-16f, 0); // Start to charge, go fast
            else
                rb2d.velocity = new Vector2(-16f, (target.position.y - transform.position.y) * 0.5f); // If later, charge the player
        }
    }

    /**
     * Stops the ship from charging if it hasn't started
     */
    public void StopRunner()
    {
        if(skullRef != null)
            Destroy(skullRef.gameObject);
        CancelInvoke("Charge");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private Rigidbody2D rb2d; // Handles velocity
    private int phase; // The current phase of the explosion
    public Sprite phase2; // Bigger explosion
    public Sprite phase3; // Biggest explosion
    private float scale; // How much to scale the explosion sprite & collider by

	/**
     * Spawn the explosion
     */
	void Start ()
    {
        rb2d = GetComponent<Rigidbody2D>();
        SoundManager.instance.PlaySingle("explosionEnemy");
        phase = 1;
        scale = 1f;
        InvokeRepeating("UpdatePhase", 0.2f, 0.2f); // Keep updating the phase
	}

    /**
     * Sets the parameters of the explosion
     * @param scaleParam The amount by which to scale the explosion
     * @param vel The velocity that the explosion should be going
     */
    public void SetParams(float scaleParam, Vector3 vel)
    {
        rb2d = GetComponent<Rigidbody2D>(); // It's here again to make sure that the rb2d is not undefined
        scale = scaleParam;
        gameObject.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(scale, scale); // Scale the sprite
        rb2d.velocity = vel;
    }

    /**
     * Updates the sprite/phase of the explosion
     */
    void UpdatePhase()
    {
        phase++;
        if (phase == 2)
        {
            gameObject.GetComponent<CircleCollider2D>().radius = (0.7f * scale);
            gameObject.GetComponent<SpriteRenderer>().sprite = phase2;
        }
        else if(phase == 3)
        {
            gameObject.GetComponent<CircleCollider2D>().radius = (0.9f * scale);
            gameObject.GetComponent<SpriteRenderer>().sprite = phase3;
        }
        else if(phase == 4)
        {
            Destroy(gameObject);
        }
    }

    /**
     * Stops the explosion from moving
     */
    public void StopMoving()
    {
        CancelInvoke("UpdatePhase");
        rb2d.velocity = new Vector3(0, 0);
    }
}

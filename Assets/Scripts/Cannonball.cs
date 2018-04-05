using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : MonoBehaviour
{
    private bool shotByPlayer;
    private bool up;
    private float xSpeed;
    private Rigidbody2D rb2d;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    public void SetParams(bool shotByPlayer, bool up, float xSpeed)
    {
        this.shotByPlayer = shotByPlayer;
        this.up = up;
        this.xSpeed = xSpeed * 0.25f; // So that they don't go flying forward
    }

    // Update is called once per frame
    void Update ()
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewPos.y > 1 || viewPos.y < 0 || viewPos.x > 1 || viewPos.x < 0)
        {
            Destroy(gameObject, 0.5f);
        }

        Move();
        //Rotate thet transform of the game object this is attached to by 45 degrees, taking into account the time elapsed since last frame.
        transform.Rotate(new Vector3(0, 0, 45) * Time.deltaTime);
    }

    void Move()
    {
        Vector2 move = new Vector2(xSpeed, up ? 2f : -2f);
        rb2d.velocity = move;
    }

    public bool getShotByPlayer()
    {
        return shotByPlayer;
    }
}

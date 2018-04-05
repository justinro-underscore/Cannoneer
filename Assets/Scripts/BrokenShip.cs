using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenShip : Ship
{
    // Use this for initialization
    void Start()
    {
        base.Instantiate();
        speed = 2;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
        CheckIfOffScreen();
    }

    void Move()
    {
        Vector2 move = new Vector2(-1, 0);
        rb2d.velocity = move * speed;
    }

    protected new void OnTriggerEnter2D(Collider2D other)
    {
        // No collisions
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float Speed = 5;
    public float JumpPower = 1;
    public Rigidbody2D rb;

    private CollisionHandler CollisionHandler;

  

    void Start()
    {
        CollisionHandler = GetComponent<CollisionHandler>();
    }


    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector2 direction = new Vector2(x, y);
        Run(direction);

        if(Input.GetButtonDown("Jump") && CollisionHandler.numOfCollisions >= 1)
        {
            Jump(Vector2.up);
		}
       
    }

	public void Jump(Vector2 up)
	{
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += up * JumpPower;
	}
   
    private void Run(Vector2 direction)
	{
        rb.AddForce(new Vector2(direction.x * Speed, 0));
	}
}

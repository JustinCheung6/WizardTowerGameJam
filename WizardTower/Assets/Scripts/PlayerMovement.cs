using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Movement
{
    //Movement designed for player (updates movement based on user input)

    private bool immobilized = false;
    private float gravityScale;

    public void Immobilize() { immobilized = true; }
    public void Mobilize() { immobilized = false; }

    protected override void OnEnable()
    {
        base.OnEnable();
        UpdateManager.um.UpdateEvent += GetInputs;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        UpdateManager.um.UpdateEvent -= GetInputs;
    }

    new private void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        groundCheck = GetComponentInChildren<GroundCheck>();

        gravityScale = rb.gravityScale;
    }

    private void GetInputs()
    {
        if (!immobilized) 
        {
            rb.gravityScale = gravityScale;
            Move(Input.GetAxisRaw("Horizontal"));
            Jump(Input.GetButtonDown("Jump"), Input.GetButton("Jump"));
        }
        else
        {
            rb.gravityScale = 0;
            Move(0);
        }
    }
}

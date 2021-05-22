using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Movement
{
    //Movement designed for player (updates movement based on user input)

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

    private void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        groundCheck = GetComponentInChildren<GroundCheck>();
    }

    private void GetInputs()
    {
        Move(Input.GetAxisRaw("Horizontal"));
        Jump(Input.GetButtonDown("Jump"), Input.GetButton("Jump"));
    }
}

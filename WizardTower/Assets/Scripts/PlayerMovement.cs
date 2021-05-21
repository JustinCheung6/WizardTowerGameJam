using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Movement
{
    public static PlayerMovement pm = null;

    protected override void OnEnable()
    {
        if (pm == null)
            pm = this;

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
        rb = GetComponent<Rigidbody2D>();
        groundCheck = GetComponentInChildren<GroundCheck>();
    }

    private void GetInputs()
    {
        Move(Input.GetAxisRaw("Horizontal"));
        Jump(Input.GetButtonDown("Jump"), Input.GetButton("Jump"));
    }
}

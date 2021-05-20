using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Movement
{
    private void OnEnable()
    {
        UpdateManager.um.UpdateEvent += GetInputs;
    }
    private void OnDisable()
    {
        UpdateManager.um.UpdateEvent -= GetInputs;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void GetInputs()
    {
        Move(Input.GetAxisRaw("Horizontal"));

        if (Input.GetButton("Jump"))
            Jump(Input.GetButtonDown("Jump"), Input.GetButton("Jump"));
    }
}

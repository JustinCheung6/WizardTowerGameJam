using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    //Base class for moving objects (ie. player and enemies)

    protected Rigidbody2D rb;
    protected GroundCheck groundCheck;

    [Header("Movement")]
    [SerializeField] private float moveAcc = 4f;
    [SerializeField] private float moveSpeed = 5f;

    private int currentJumps = 0;
    private float jumpTime = 0f; //Time that user has held the 'jump button'
    private bool jumping = false; //If player is holding jump button

    [Header("Jumping")]
    [SerializeField] private int maxJumps = 1;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] [Tooltip("How long can the user hold the 'jump button'")] 
    private float maxJumpTime = 0.5f;

    protected void Move(float direction)
    {
        float move = rb.velocity.x;

        if (direction > 0)
            move = Mathf.Clamp(move + (moveAcc * Time.fixedDeltaTime), moveAcc, moveSpeed);
        else if (direction < 0)
            move = Mathf.Clamp(move - (moveAcc * Time.fixedDeltaTime), -moveSpeed, -moveAcc);
        else
            move = 0;

        rb.velocity = new Vector3(move, rb.velocity.y);
    }
    protected void Jump(bool jumpPress, bool jumpDown)
    {
        CheckGround();

        if (jumpPress && currentJumps > 0)
        {
            currentJumps -= 1;
            jumping = true;
            jumpTime = 0;
        }
        else if(jumpDown && jumping && (jumpTime + Time.deltaTime) <= maxJumpTime && rb.velocity.y > 0)
        {
            jumpTime += Time.deltaTime;
        }
        else
        {
            jumping = false;
            jumpTime = 0;
            return;
        }

        rb.velocity = new Vector3(rb.velocity.x, jumpForce);
    }

    private void CheckGround()
    {
        if (groundCheck.IsGrounded)
            currentJumps = maxJumps;
        else if (currentJumps == maxJumps)
            currentJumps = maxJumps - 1;
    }
}

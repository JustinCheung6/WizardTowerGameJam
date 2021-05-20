using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    //Base class for moving objects (ie. player and enemies)

    private Rigidbody2D rb;

    private float moveAcc = 4f;
    private float moveSpeed = 5f;

    private float jumpTime = 0f;

    private GroundCheck groundCheck;
    private bool jumping = false;
    private bool jumpRecently = false; //Prevents Player from jumping on next update frame if they've jumped already
    private float jumpForce = 5f;
    private float maxJumpTime = 1f; //How long can the player hold the jump button

    protected void Move(int direction)
    {
        float move = rb.velocity.x;

        if (direction > 0)
            move = Mathf.Clamp(move + moveAcc, moveAcc, moveSpeed);
        else if (direction < 0)
            move = Mathf.Clamp(move - moveAcc, -moveSpeed, -moveAcc);
        else
            move = 0;

        rb.velocity = new Vector3(move, rb.velocity.y);
    }
    protected void Jump(bool jumpPress, bool jumpDown)
    {
        if (jumpPress && groundCheck.IsGrounded && !jumpRecently)
        {
            jumping = true;
            jumpRecently = true;
        }
        else if(jumpDown && jumping && (jumpTime + Time.deltaTime) <= maxJumpTime && rb.velocity.y > 0)
        {
            jumpTime += Time.deltaTime;
        }
        else
        {
            jumping = false;
            jumpTime = 0;
            jumpRecently = false;
            return;
        }

        rb.velocity = new Vector3(rb.velocity.x, jumpForce);
        jumpRecently = false;
    }


}

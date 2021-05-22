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

    [Header("Jumping")]
    [SerializeField] private int maxJumps = 1;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] [Tooltip("How long can the user hold the 'jump button'")] 
    private float maxJumpTime = 0.5f;

    [Header("Debugging (Don't Touch)")]
    [SerializeField] private int currentJumps = 0;
    [SerializeField] private float jumpTime = 0f; //Time that user has held the 'jump button'
    [SerializeField] private bool jumping = false; //If player is holding jump button

    public Vector3 Velocity { get => rb.velocity; }
    public bool IsGrounded { get => groundCheck.IsGrounded; }

    protected virtual void OnEnable()
    {
        UpdateManager.um.UpdateEvent += CheckGround;
    }
    protected virtual void OnDisable()
    {
        UpdateManager.um.UpdateEvent -= CheckGround;
    }

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
        float jump = jumpForce;

        if (jumpPress && currentJumps > 0)
        {
            Debug.Log("Jump");
            currentJumps -= 1;
            jumping = true;
            jumpTime = 0;
        }
        else if(jumpDown && jumping && (jumpTime + Time.deltaTime <= maxJumpTime) && rb.velocity.y > 0)
        {
            jumpTime += Time.deltaTime;
            //jump += jumpTime * jumpForce;
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

    public int GetDirection()
    {
        if (rb.velocity.x > 0)
            return 1;
        else if (rb.velocity.x < 0)
            return -1;
        else
            return 0;
    }
}

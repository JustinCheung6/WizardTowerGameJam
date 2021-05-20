using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    #region Customizable Variables
    [SerializeField] protected float minRoamRange = 10f;
    [SerializeField] protected float maxRoamRange = 50f;
    [SerializeField] protected float idleTime = 2f;
    [SerializeField] protected float roamSpeed = 1f;
    [SerializeField] protected float chaseSpeed = 2f;
    [SerializeField] protected float chaseJumpForce = 200f;
    [SerializeField] protected float minCaptureRange = 2.5f;
    [SerializeField] protected float captureExtraReactionTime = 0.5f;
    #endregion

    #region State Management Variables
    protected enum State { 
        Roaming, Idling, Chasing, Attacking
    }
    protected State state;
    #endregion

    #region Roaming Variables
    protected Vector3 startingPosition;
    protected Vector2 roamingPosition;
    protected bool roamingToRight;
    #endregion

    #region Idling Variables
    protected float idleStartTime;
    #endregion

    #region Chasing Variables
    [HideInInspector] public GameObject chasedObject;
    #endregion

    #region Capture Variables
    protected float captureStartTime;
    #endregion

    #region Reference Variables
    protected Rigidbody2D rb2d;
    protected SpriteRenderer sr;
    protected Animator anim;
    [SerializeField] protected GameObject CaptureCollider;
    #endregion

    protected void Start()
    {
        rb2d = this.gameObject.GetComponent<Rigidbody2D>();
        sr = this.gameObject.GetComponent<SpriteRenderer>();
        anim = this.gameObject.GetComponent<Animator>();

        startingPosition = this.transform.position;
        roamingPosition = GetRoamingPosition();
        state = State.Roaming;
        SetRoamDirection();
    }

    /*    protected void OnEnable()
        {
            UpdateManager.um.FixedUpdateEvent += MoveToPosition;
        }

        protected void OnDisable()
        {
            UpdateManager.um.FixedUpdateEvent -= MoveToPosition;
        }*/

    protected void FixedUpdate()
    {
        StateManager();
    }

    protected void StateManager() {
        switch (state) {
            case State.Roaming:
                if (anim.GetBool("isAttacking"))
                    anim.SetBool("isAttacking", false);
                MoveToPosition(roamSpeed,0);
                break;
            case State.Idling:
                if (anim.GetBool("isRunning"))
                    anim.SetBool("isRunning", false);
                
                if (minRoamRange != 0 && maxRoamRange != 0) {
                    if (Time.time - idleStartTime >= idleTime) {
                        roamingPosition = GetRoamingPosition();
                        SetRoamDirection();
                        state = State.Roaming;
                    }
                }
                break;
            case State.Chasing:
                if (!anim.GetBool("isRunning"))
                    anim.SetBool("isRunning", true);
                if (chasedObject != null)
                {
                    if (Mathf.Abs(chasedObject.transform.position.x - rb2d.position.x) > minCaptureRange)
                    {
                        roamingPosition = chasedObject.transform.position;
                        MoveToPosition(chaseSpeed, chaseJumpForce);
                    }
                    else
                    {
                        state = State.Attacking;
                        captureStartTime = Time.time;
                    }
                }
                else {
                    TriggerIdleState();
                }
                break;
            case State.Attacking:
                if (chasedObject.CompareTag("Player")) {
                    //if (Time.time - captureStartTime >= captureExtraReactionTime) {
                        anim.Play("attack");
                    //}
                }
                TriggerIdleState();
                break;
            default:
                break;
        }
    }

    protected void ChaseObject() {
        rb2d.MovePosition(rb2d.position + new Vector2(chaseSpeed, 0) * Time.fixedDeltaTime);
    }

    protected void MoveToPosition(float speedX, float jumpForce) {
        if ((roamingToRight && rb2d.position.x >= roamingPosition.x) || (!roamingToRight && rb2d.position.x <= roamingPosition.x)) {
            anim.SetBool("isRunning", false);
            TriggerIdleState();
        }
        else
        {
            if (!anim.GetBool("isRunning"))
                anim.SetBool("isRunning", true);
            if (roamingToRight && rb2d.velocity.y == 0)
                //rb2d.MovePosition(rb2d.position + new Vector2(speedX, 0) * Time.fixedDeltaTime);
                rb2d.velocity = new Vector2(speedX, 0);
            else if(!roamingToRight && rb2d.velocity.y == 0)
                //rb2d.MovePosition(rb2d.position + new Vector2(speedX * -1, 0) * Time.fixedDeltaTime);
                rb2d.velocity = new Vector2(speedX * -1, 0);
            if (chasedObject != null) {
                if(chasedObject.transform.position.y > rb2d.position.y && rb2d.velocity.y == 0)
                {
                    rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce);
                }
            }
        }
    }

    // Get a random roaming position
    protected Vector2 GetRoamingPosition() {
        Vector2 roamVector = new Vector2(startingPosition.x + GetRandomXDirection() * Random.Range(minRoamRange, maxRoamRange), rb2d.position.y);
        float direction = roamVector.x - rb2d.position.x;
        if ((direction < 0 && transform.localScale.x > 0) || (direction > 0 && transform.localScale.x < 0))
            transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);
        return roamVector;
    }

    protected void SetRoamDirection() {
        if (rb2d.position.x > roamingPosition.x)
            roamingToRight = false;
        else if (rb2d.position.x < roamingPosition.x)
            roamingToRight = true;
        else
        {
            TriggerIdleState();
        }
    }

    // Returns a random X direction for the AI to travel in
    public static float GetRandomXDirection() { 
        return UnityEngine.Random.Range(-1f, 1f);
    }

    public void TriggerChaseState() {
        state = State.Chasing;
    }
    public void TriggerIdleState() {
        state = State.Idling;
        idleStartTime = Time.time;
        
    }
}
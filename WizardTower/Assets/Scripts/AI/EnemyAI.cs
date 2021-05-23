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
    [SerializeField] protected float runTimeBeforeTurnAround = 1.5f;
    [SerializeField] protected float minCaptureRange = 2.5f;
    [SerializeField] protected float captureExtraReactionTime = 0.5f;
    
    #endregion

    #region State Management Variables
    protected enum State { 
        Roaming, Idling, Chasing, Attacking, Stunned, TurnAndChase, Null
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
    [HideInInspector] public bool turnAndChaseRequested = false;
    protected float turnAndChaseStartTime = 0f;
    #endregion

    #region Capture Variables
    protected float captureStartTime;
    #endregion

    #region Stunned Variables
    protected bool isStunned = false;
    //protected bool isStunnedBool = false;
    protected float stunDuration = 0f;
    protected float stunStartTime;
    #endregion

    #region Reference Variables
    protected Rigidbody2D rb2d;
    protected SpriteRenderer sr;
    protected Animator anim;
    [SerializeField] protected GameObject CaptureCollider;
    [SerializeField] protected GameObject LineOfSight;
    protected State returnToState = State.Null;
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
        if (isStunned)
            state = State.Stunned;
        if (turnAndChaseRequested)
            state = State.TurnAndChase;
    }

    protected void StateManager() {
        switch (state) {
            case State.Roaming:
                Debug.Log("Roaming");
                CheckForStun(State.Roaming);
                if (anim.GetBool("isAttacking"))
                    anim.SetBool("isAttacking", false);
                MoveToPosition(roamSpeed,0);
                break;
            case State.Idling:
                Debug.Log("Idling");
                CheckForStun(State.Idling);
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
                Debug.Log("Chasing");
                CheckForStun(State.Chasing);
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
                Debug.Log("Attacking");
                CheckForStun(State.Attacking);
                if (chasedObject.CompareTag("Player"))
                {
                    if (Time.time - captureStartTime >= captureExtraReactionTime)
                    {
                        anim.Play("attack");
                        TriggerIdleState();
                    }
                }
                else {
                    TriggerIdleState();
                }
                break;
/*            case State.Stunned:
                Debug.Log("Stunned");
                if (!isStunned)
                {
                    if(!anim.GetBool("isStunned"))
                        anim.SetBool("isStunned", true);
                    isStunned = true;
                    stunStartTime = Time.time;
                    LineOfSight.SetActive(false);
                }
                else {
                    if (Time.time - stunStartTime >= stunDuration) {
                        isStunned = false;
                        anim.SetBool("isStunned", false);
                        LineOfSight.SetActive(true);
                        TriggerIdleState();
                    }
                }
                break;*/
            case State.Stunned:
                Debug.Log("Stunned");
                if (isStunned)
                {
                    if (!anim.GetBool("isStunned")) {
                        anim.SetBool("isStunned", true);
                        LineOfSight.SetActive(false);
                        if (stunDuration > 0f)
                            stunStartTime = Time.time;
                    }
                    if (stunDuration > 0f) { // Disable by time
                        if (Time.time - stunStartTime >= stunDuration) {
                            if (anim.GetBool("isStunned"))
                            {
                                isStunned = false;
                                anim.SetBool("isStunned", false);
                                LineOfSight.SetActive(true);
                                stunDuration = 0f;
                                TriggerIdleState();
                            }
                        }
                    }
                }
                else // Disables if the bool is called; will override the time (if enemy gets "unstunned" before the 
                     // timer runs out for Yogurt, then the timer is now irrelevant and the enemy is unstunned)
                {
                    if (anim.GetBool("isStunned")) {
                        anim.SetBool("isStunned", false);
                        LineOfSight.SetActive(true);
                        stunDuration = 0f;
                        if (returnToState == State.Null)
                            TriggerIdleState();
                        else
                            state = returnToState;
                    }
                }
                break;
            case State.TurnAndChase:
                Debug.Log("Turn and Chasing");
                CheckForStun(State.TurnAndChase);
                if (turnAndChaseRequested)
                {
                    turnAndChaseRequested = false;
                    turnAndChaseStartTime = Time.time;
                }
                else if (!turnAndChaseRequested && Time.time - turnAndChaseStartTime >= runTimeBeforeTurnAround)
                {
                    // Exclaimation animation here
                    if (chasedObject == null)
                    {
                        transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);
                        if (chasedObject == null)
                        {
                            MoveToPosition(roamSpeed, 0);
                            state = State.Roaming;
                        }
                    }
                }
                else if (!turnAndChaseRequested && Time.time - turnAndChaseStartTime < runTimeBeforeTurnAround) {
                    MoveToPosition(roamSpeed,0);
                }
                    break;
            default:
                break;
        }
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
                float direction = chasedObject.transform.position.x - rb2d.position.x;
                if ((direction < 0 && transform.localScale.x > 0) || (direction > 0 && transform.localScale.x < 0))
                    transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);
                if (chasedObject.transform.position.y > rb2d.position.y && rb2d.velocity.y == 0)
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

    protected void CheckForStun(State s) {
        if (anim.GetBool("isStunned") && !isStunned) {
            returnToState = s;
            state = State.Stunned;
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
    public void TriggerStunState(float stunDuration)
    {
        if (!isStunned) {
            isStunned = true;
            this.stunDuration = stunDuration;
            state = State.Stunned;
        }
    }
    public void TriggerStunState(bool stunRequest)
    {
        if (stunRequest)
            isStunned = true;
        else
            isStunned = false;
        state = State.Stunned;
    }
}

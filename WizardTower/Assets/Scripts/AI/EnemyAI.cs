using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    #region Customizable Variables
    [SerializeField] protected float minRoamRange = 10f;
    [SerializeField] protected float maxRoamRange = 50f;
    [SerializeField] protected float idleTime = 2f;
    [SerializeField] protected float roamSpeed = 10f;
    [SerializeField] protected float chaseSpeed = 15f;
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

    #region Reference Variables
    protected Rigidbody2D rb2d;
    protected SpriteRenderer sr;
    protected Animator anim;
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
                MoveToPosition();
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
                Debug.Log("Chasing!!!");
                break;
            default:
                break;
        }
    }

    protected void MoveToPosition() {
        if ((roamingToRight && rb2d.position.x >= roamingPosition.x) || (!roamingToRight && rb2d.position.x <= roamingPosition.x)) {
            anim.SetBool("isRunning", false);
            state = State.Idling;
            idleStartTime = Time.time;
        }
        else
        {
            if (!anim.GetBool("isRunning"))
                anim.SetBool("isRunning", true);
            if (roamingToRight)
                rb2d.MovePosition(rb2d.position + new Vector2(roamSpeed, 0) * Time.fixedDeltaTime);
            else
                rb2d.MovePosition(rb2d.position + new Vector2(roamSpeed*-1, 0) * Time.fixedDeltaTime);
        }
    }

    // Get a random roaming position
    protected Vector2 GetRoamingPosition() {
        
            
        Vector2 roamVector = new Vector2(startingPosition.x + GetRandomXDirection() * Random.Range(minRoamRange, maxRoamRange), startingPosition.y);
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
            state = State.Idling;
            idleStartTime = Time.time;
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
    }
}

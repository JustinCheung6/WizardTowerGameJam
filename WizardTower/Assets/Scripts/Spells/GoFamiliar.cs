using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoFamiliar : Spell
{
    //Spell that will makes a distraction when it hits the floor, and stuns the gaurd if it hits them

    #region Components
    private Rigidbody2D rb = null;
    private SpriteRenderer sprite = null;
    private CircleCollider2D noiseCol = null;
    #endregion

    #region Customizable Variables
    [Header("Go Familiar")]
    [SerializeField] private Vector2 summonOffset = new Vector2();
    [SerializeField] private float throwForce = 5f;
    [SerializeField] private float throwAngle = 45f;
    [SerializeField] private float noiseTime = 2f;
    [SerializeField] private float stunTime = 2f;
    #endregion

    private Vector2 forceDirection = new Vector2();
    private float timer = 0f;
    //Physics layers needed in script (switches between the two)
    [SerializeField] private int spellLayer;
    [SerializeField] private int idleLayer; 


    public bool MakingNoise { get => timer > 0f; }

    private void Start()
    {
        spellLayer = LayerMask.NameToLayer("Spell");
        gameObject.layer = idleLayer = LayerMask.NameToLayer("SpellIdle");

        forceDirection = new Vector2(Mathf.Cos((Mathf.PI * throwAngle) / 180), 
            Mathf.Sin((Mathf.PI * throwAngle) / 180));
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        foreach (CircleCollider2D c in GetComponentsInChildren<CircleCollider2D>())
        {
            if (c.isTrigger)
            {
                noiseCol = c;
                break;
            }
        }


    }
    public override bool CanCast()
    {
        return true;
    }

    public override void CastSpell(bool press, bool down, bool release)
    {
        if (!press)
            return;

        casting = true;

        ReturnFamiliar(Player.p.LookingRight);
        CastFamiliar(Player.p.LookingRight);
        casted = true;
    }

    private void ReturnFamiliar(bool isRight)
    {
        int direction = (isRight) ? 1 : -1;
        transform.position = Player.p.transform.position;
        transform.position += new Vector3(direction * summonOffset.x, summonOffset.y);
    }

    private void CastFamiliar(bool isRight)
    {
        if (!sprite.enabled)
            sprite.enabled = true;

        gameObject.layer = spellLayer;

        int direction = (isRight) ? 1 : -1;

        rb.velocity = new Vector2(direction * forceDirection.x, forceDirection.y) * throwForce;
        if (Player.pm.GetDirection() == direction)
            rb.velocity += new Vector2(Player.pm.Velocity.x, 0f);
    }

    private void MakeNoise()
    {
        timer += Time.deltaTime;
        if(timer >= noiseTime)
        {
            casted = false;
            UpdateManager.um.UpdateEvent -= MakeNoise;
        }
    }

    public override void ResetSpell()
    {
        noiseCol.enabled = false;
        timer = 0;
        casted = false;
    }

    public void OnCollisionEnter2D(Collision2D c)
    {
        if (!casting)
            return;

        if(c.gameObject.CompareTag("Enemy"))
        {
            gameObject.layer = idleLayer;

            c.gameObject.GetComponent<EnemyAI>().TriggerStunState(stunTime, true);
            casting = false;
            casted = false;
        }

    }
    public void OnTriggerEnter2D(Collider2D c)
    {
        if (!casting)
            return;

        if (c.gameObject.CompareTag("Floor"))
        {
            gameObject.layer = idleLayer;

            Debug.Log("Yogurt Floor");
            casting = false;
            noiseCol.enabled = true;
            UpdateManager.um.UpdateEvent += MakeNoise;
        }
    }
}

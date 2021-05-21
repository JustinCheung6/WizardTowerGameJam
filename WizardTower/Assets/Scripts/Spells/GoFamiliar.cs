using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoFamiliar : Spell
{
    //Spell that will makes a distraction when it hits the floor, and stuns the gaurd if it hits them

    private Rigidbody2D rb = null;
    private SpriteRenderer sprite = null;
    private Collider2D col = null;
    private CircleCollider2D noiseCol = null;

    private Vector2 forceDirection = new Vector2();

    [SerializeField] private Vector2 summonOffset = new Vector2();
    [SerializeField] private float throwForce = 5f;
    [SerializeField] private float throwAngle = 45f;
    [SerializeField] private float noiseTime = 2f;

    private float timer = 0f;


    public bool MakingNoise { get => timer > 0f; }

    private void Start()
    {
        forceDirection = new Vector2(Mathf.Cos((Mathf.PI * throwAngle) / 180), 
            Mathf.Sin((Mathf.PI * throwAngle) / 180));
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        foreach(CircleCollider2D c in GetComponentsInChildren<CircleCollider2D>())
        {
            if (c != col)
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

        bool direction = !Player.p.SpriteRen.flipX;

        ReturnFamiliar(direction);
        CastFamiliar(direction);
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
        int direction = (isRight) ? 1 : -1;

        rb.velocity = new Vector2(direction * forceDirection.x, forceDirection.y) * throwForce;
        if (PlayerMovement.pm.GetDirection() == direction)
            rb.velocity += new Vector2(PlayerMovement.pm.Velocity.x, 0f);
    }

    private void MakeNoise()
    {
        
        timer += Time.deltaTime;
        if(timer >= noiseTime)
        {
            noiseCol.enabled = false;
            casting = false;
            timer = 0;
            UpdateManager.um.UpdateEvent -= MakeNoise;
        }
    }

    public override void ResetSpell()
    {
        casted = false;
    }

    public void OnCollisionEnter2D(Collision2D c)
    {
        if(c.gameObject.tag == "Enemy")
        {
            casting = false;
            col.enabled = false;
        }
    }
    public void OnTriggerEnter2D(Collider2D c)
    {
        if (c.tag == "Floor")
        {
            col.enabled = false;
            noiseCol.enabled = true;
            UpdateManager.um.UpdateEvent += MakeNoise;
        }
    }
}

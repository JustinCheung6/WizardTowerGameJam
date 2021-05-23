using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyString : Spell
{
    private SpriteRenderer sprite = null;

    [SerializeField] private Vector2 castRange = new Vector2(25f, 99999f);

    private Transform target = null;
    private float stringSize = 0;

    //Guard
    private bool flingLeft = false;

    [Header("Ceiling Grab")]
    [SerializeField] private float upSpeed = 10f;
    [Tooltip("How far will player be from ceiling after being launched")]
    [SerializeField] private float ceilingOffset = 1f;

    private void Start()
    {
        stringSize = transform.localScale.x;
        sprite = GetComponent<SpriteRenderer>();
    }

    public override bool CanCast()
    {
        return (RaycastObject(Input.GetAxis("Vertical") > 0) != null);
    }

    public override void CastSpell(bool press, bool down, bool release)
    {
        if (!press)
            return;
        
        casting = true;

        if (Input.GetAxis("Vertical") > 0)
        {
            target = RaycastObject(true);

            Player.pm.Immobilize();
            sprite.enabled = true;
            casted = true;
            casting = false;

            UpdateManager.um.FixedUpdateEvent += ShootUp;
        }
        else if (Player.pm.IsGrounded)
        {
            //SET GUARDS' VELOCITY TO FLING TOWARDS PLAYER (also stop its regular movement)
            target = RaycastObject(false);
            EnemyAI targetEnemyAI = target.gameObject.GetComponent<EnemyAI>();
            if (targetEnemyAI != null) 
                targetEnemyAI.TriggerStunState(true);
            //flingLeft = Player.p.transform.position.x < target.position.x;

            Player.pm.Immobilize();
            sprite.enabled = true;
            casted = true;
            casting = false;

            UpdateManager.um.FixedUpdateEvent += DragGuard;
        }
    }

    private void ShootUp()
    {
        float travel = upSpeed * Time.fixedDeltaTime;

        if (Player.p.transform.position.y + travel < target.position.y - ceilingOffset)
        {
            Player.p.transform.position += new Vector3(0f, travel);
        }
        else
        {
            Player.p.transform.position = new Vector3(Player.p.transform.position.x, target.position.y - ceilingOffset);
        }

        transform.position = new Vector3(Player.p.transform.position.x, 
            Player.p.transform.position.y + (target.position.y - Player.p.transform.position.y) /2 );
        transform.localScale = new Vector3(stringSize,
            target.position.y - Player.p.transform.position.y);

        if (Player.p.transform.position.y + travel >= target.position.y - ceilingOffset)
        {
            casted = false;
            UpdateManager.um.FixedUpdateEvent -= ShootUp;
        }

    }
    private void DragGuard()
    {
        if ( (Player.p.transform.position.x >= target.position.x && flingLeft) ||
            (Player.p.transform.position.x <= target.position.x && !flingLeft))
        {
            casted = false;
            //REENABLE GUARD'S MOVEMENT HERE, AND STOP EXTRA VELOCITY (Have Guard chase opposite direction)
            EnemyAI targetEnemyAI = target.gameObject.GetComponent<EnemyAI>();
            if (targetEnemyAI != null)
                targetEnemyAI.TriggerStunState(false);
            UpdateManager.um.FixedUpdateEvent -= DragGuard;
            return;
        }
        else if(Player.p.transform.position.x > target.position.x)
        {
            transform.localScale = new Vector3(target.position.x - Player.p.transform.position.x, 
                stringSize);
            transform.position = new Vector3(target.position.x + 
                (Player.p.transform.position.x - target.position.x) / 2, Player.p.transform.position.y);

        }
        else if (Player.p.transform.position.x < target.position.x)
        {
            transform.localScale = new Vector3( Player.p.transform.position.x - target.position.x,
                stringSize);
            transform.position = new Vector3(Player.p.transform.position.x +
                 (target.position.x - Player.p.transform.position.x) / 2, Player.p.transform.position.y);
        }
    }

    public override void ResetSpell()
    {
        sprite.enabled = false;
        Player.pm.Mobilize();
        target = null;
        transform.localScale = new Vector3(stringSize, stringSize);

        casting = false;
        casted = false;
    }

    private Transform RaycastObject(bool vertical)
    {
        if (vertical)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(Player.p.transform.position, Vector2.up, castRange.y);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform.CompareTag("Ceiling"))
                    return hit.transform;
            }
            return null;
        }
        else
        {
            Vector2 direction = (Player.p.transform.localScale.x > 0) ? Vector2.right : Vector2.left;

            RaycastHit2D[] hits = Physics2D.RaycastAll(Player.p.transform.position, direction, castRange.y);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform.CompareTag("Enemy"))
                    return hit.transform;
            }
            return null;
        }
       
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switcharoo : Spell
{
    private SpriteRenderer sprite = null;
    private Collider2D col = null;

    [Header("Switcharoo")]
    [SerializeField] private float onDuration = 1f;
    [SerializeField] private float offCooldown = 2f;

    private float timer = 0f;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        sprite.enabled = false;
        col.enabled = false;
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
        sprite.enabled = true;
        col.enabled = true;
        UpdateManager.um.UpdateEvent += SwitchReady;
    }

    public override void ResetSpell()
    {
        timer = 0f;
        sprite.enabled = false;
        col.enabled = false;

        casting = false;
        casted = false;
    }

    private void SwitchReady()
    {
        if(timer >= onDuration)
        {
            casting = false;
            UpdateManager.um.UpdateEvent -= SwitchReady;
            ExtendCooldown();
            ResetSpell();
            return;
        }

        timer += Time.deltaTime;
        transform.position = Player.p.transform.position;
    }

    private void ExtendCooldown()
    {
        if (!SpellManager.sm.IsCurrentSpell(this))
            return;

        SpellManager.sm.ExtendCooldown(offCooldown);
    }

    private void OnCollisionEnter2D(Collision2D c)
    {
        if (!casting)
            return;

        if (c.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Switch Collision");

            casting = false;
            casted = true;

            UpdateManager.um.UpdateEvent -= SwitchReady;

            //Switch enemy and player positions
            Vector3 playerPos = Player.p.transform.position;
            c.gameObject.SetActive(false);
            Player.p.transform.position = c.transform.position;
            c.transform.position = playerPos;
            c.gameObject.SetActive(true);

            //Enemy Confusion (Turn around & chase)
            c.gameObject.GetComponent<EnemyAI>().turnAndChaseRequested = true;

            casted = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D c)
    {
        if (!casting)
            return;

        if (c.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Switch Collision");

            casting = false;
            casted = true;

            UpdateManager.um.UpdateEvent -= SwitchReady;

            //Switch enemy and player positions
            Vector3 playerPos = Player.p.transform.position;
            c.gameObject.SetActive(false);
            Player.p.transform.position = c.transform.position;
            c.transform.position = playerPos;
            c.gameObject.SetActive(true);

            //Enemy Confusion (Turn around & chase)
            c.gameObject.GetComponent<EnemyAI>().turnAndChaseRequested = true;

            SpellManager.sm.ManualPop(this);
            casted = false;
        }
    }
}

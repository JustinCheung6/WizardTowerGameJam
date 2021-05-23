using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Class that controls player aside from movement script

    public static Player p = null;
    public static PlayerMovement pm = null;

    private SpriteRenderer sprite = null;
    private Animator anim = null;

    public SpriteRenderer SpriteRen { get => sprite; }
    public Animator Animator { get => anim; }
    public bool LookingRight { get => transform.localScale.x > 0; }

    private void OnEnable()
    {
        if (p == null)
            p = this;
        if (pm == null)
            pm = this.GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
    }
}

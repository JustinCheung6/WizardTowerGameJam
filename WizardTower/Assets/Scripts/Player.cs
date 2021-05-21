using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player p = null;

    private SpriteRenderer sprite = null;

    public SpriteRenderer SpriteRen { get => sprite; }

    private void OnEnable()
    {
        if (p == null)
            p = this;
    }

    private void Start()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
    }
}

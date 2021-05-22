using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell : MonoBehaviour
{
    //Base abstract class for all Spells
    
    [SerializeField] private float cooldown = 5f;
    protected bool casting = false;
    [SerializeField] protected bool casted = false;

    public float Cooldown { get => cooldown; }
    public bool SpellCasted { get => casted; }
    public bool CastingSpell { get => casting; }



    public abstract void CastSpell(bool press, bool down, bool release);

    public abstract bool CanCast();

    public abstract void ResetSpell();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyString : Spell
{
    public override bool CanCast()
    {
        return true;
    }

    public override void CastSpell(bool press, bool down, bool release)
    {
        if (!press)
            return;

    }

    public override void ResetSpell()
    {
        
    }
}

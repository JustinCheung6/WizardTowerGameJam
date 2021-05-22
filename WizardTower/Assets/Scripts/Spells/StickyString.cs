using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyString : Spell
{
    [SerializeField] private Vector2 castRange = new Vector2(25f, 99999f);

    [SerializeField] private float launchTime = 1f;


    public override bool CanCast()
    {
        if (Input.GetAxis("Vertical") > 0)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(Player.p.transform.position, Vector2.up, castRange.y);
            foreach(RaycastHit2D hit in hits)
            {
                if (hit.transform.CompareTag("Ceiling"))
                    return true;
            }
            return false;
        }



        //Check which value is down, vertical or horizontal

        //Vertical: Check if wall is in range

        //Horizontal: Check if guard is in range

        return true;
    }

    public override void CastSpell(bool press, bool down, bool release)
    {
        if (press)
        {
            //Check if vertical or horizontal
            //Vertical: launch player up to ceiling, set casting true immobolize player
            //Horizontal: launch guard to player, imobolize player until firing done (Guard should be at player when cast is done) (can cast spells during)
        }

    }

    private void ShootUp()
    {
        //Have string connect to ceiling
        //Immobolize player (no input)
        //Update player position until it reaches destination, and update string to look attached to player
    }
    private void DragGuard()
    {
        //Set velocitty of guard to player
        //Have string match guard until certain time has passed. Also immobilize player
    }

    public override void ResetSpell()
    {
        casting = false;
        casted = false;
    }
}

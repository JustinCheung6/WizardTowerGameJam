using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    //GroundCheck for Movement class's Jump function. Sees if movement object's ready to jump
    
    private int groundCount = 0;

    //List of colliders the GroundCheck is currently touching
    private Dictionary<int, GameObject> colliders;

    //If a collider has been removed from colliders recently (is true when CheckGround is added as to delegate event)
    private bool colliderRemoved = true;
    //Last y coordinate of player when performing CheckGround()
    private float yAxis = 9999999999;

    public bool IsGrounded { get => groundCount > 0; }

    private void CheckGround()
    {
        //If the groundcheck never goes lower than it's last y axis and if no colliders were removed, 
        //the object should till be on the ground
        if (transform.position.y >= yAxis && !colliderRemoved)
            return;

        int newCount = 0; 

        foreach(GameObject platform in colliders.Values)
        {
            if (transform.position.y >= platform.transform.position.y)
                newCount++;
        }

        groundCount = newCount;
        yAxis = transform.position.y;
        colliderRemoved = false;
    }

    private void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.tag == "Floor")
        {
            if(colliders.Count == 0)
            {
                colliders.Add(c.gameObject.GetInstanceID(), c.gameObject);
                UpdateManager.um.UpdateEvent += CheckGround;
            }
            else if (!colliders.ContainsKey(c.gameObject.GetInstanceID()))
                colliders.Add(c.gameObject.GetInstanceID(), c.gameObject);

        }
    }
    private void OnCollisionExit2D(Collision2D c)
    {
        if (c.gameObject.tag == "Floor")
        {
            if(colliders.ContainsKey(c.gameObject.GetInstanceID()))
                colliders.Remove(c.gameObject.GetInstanceID());

            if(colliders.Count == 0)
                UpdateManager.um.UpdateEvent -= CheckGround;
        }
    }
}

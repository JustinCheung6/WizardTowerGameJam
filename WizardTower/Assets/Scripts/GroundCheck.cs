using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    //GroundCheck for Movement class's Jump function. Sees if movement object's ready to jump
    [SerializeField] private int groundCount = 0;
    public bool IsGrounded { get => groundCount > 0; }

    private void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject.tag == "Floor")
            groundCount += 1;
    }
    private void OnTriggerExit2D(Collider2D c)
    {
        if (c.gameObject.tag == "Floor")
        {
            groundCount -= 1;
            if (groundCount < 0)
                groundCount = 0;
        }
    }

}

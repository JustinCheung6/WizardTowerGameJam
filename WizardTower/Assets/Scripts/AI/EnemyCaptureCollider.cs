using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCaptureCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Player hit! Dory is now captured!");
        if (collision.CompareTag("Player")) {
            collision.tag = "Distraction";
            GameHandler.SignalDoryCapture();
            Player.p.Animator.Play("captured");
        }
    }
}

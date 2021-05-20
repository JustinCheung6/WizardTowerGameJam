using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLineOfSight : MonoBehaviour
{
    [SerializeField] protected EnemyAI associatedAI;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Distraction")) {
            associatedAI.chasedObject = collision.gameObject;
            associatedAI.TriggerChaseState();
        }  
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Distraction")) {
            associatedAI.chasedObject = null;
            associatedAI.TriggerIdleState();
        }
            
    }
}

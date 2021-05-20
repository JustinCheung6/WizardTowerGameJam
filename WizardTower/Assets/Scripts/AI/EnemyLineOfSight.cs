using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLineOfSight : MonoBehaviour
{
    [SerializeField] protected EnemyAI associatedAI;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
            associatedAI.TriggerChaseState();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            associatedAI.TriggerIdleState();
            Debug.Log("Player exited line of sight. Now idling.");
        }
            
    }
}

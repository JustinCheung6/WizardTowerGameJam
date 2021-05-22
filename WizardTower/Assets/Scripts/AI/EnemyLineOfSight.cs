using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLineOfSight : MonoBehaviour
{
    [SerializeField] protected EnemyAI associatedAI;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Found " + collision.tag);
        if (collision.CompareTag("Player") || collision.CompareTag("Distraction"))
        {
            
            if (associatedAI.chasedObject != null)
            {
                if (!associatedAI.chasedObject.CompareTag("Player"))
                    associatedAI.chasedObject = collision.gameObject;
            }
            else
            {
                associatedAI.chasedObject = collision.gameObject;
            }
            associatedAI.TriggerChaseState();
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("Found " + collision.tag);
        if (collision.CompareTag("Player") || collision.CompareTag("Distraction")) {
            
            if (associatedAI.chasedObject != null)
            {
                if (!associatedAI.chasedObject.CompareTag("Player"))
                    associatedAI.chasedObject = collision.gameObject;
            }
            else {
                associatedAI.chasedObject = collision.gameObject;
            }
            associatedAI.TriggerChaseState();
        }  
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Distraction")) {
            if(!associatedAI.turnAndChaseRequested)
                associatedAI.chasedObject = null;
            associatedAI.TriggerIdleState();
        }
            
    }
}

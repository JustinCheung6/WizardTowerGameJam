using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private void OnEnable()
    {
        UpdateManager.um.UpdateEvent += FollowPlayer;
    }
    private void OnDisable()
    {
        UpdateManager.um.UpdateEvent -= FollowPlayer;
    }

    private void FollowPlayer()
    {
        transform.position = new Vector3(Player.p.transform.position.x, transform.position.y, -10f);
    }
}

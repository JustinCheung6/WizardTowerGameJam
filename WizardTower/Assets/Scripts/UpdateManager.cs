using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{
    //This class holds delegate events for Unity functions such as Update

    public static UpdateManager um = null;

    public delegate void OnUpdate();
    public event OnUpdate UpdateEvent;
    public delegate void OnFixedUpdate();
    public event OnFixedUpdate FixedUpdateEvent;

    private void OnEnable()
    {
        if (um == null)
            um = this;
    }

    private void Update()
    {
        if(UpdateEvent != null)
            UpdateEvent();
    }
    private void FixedUpdate()
    {
        if (FixedUpdateEvent != null)
            FixedUpdateEvent();
    }
}

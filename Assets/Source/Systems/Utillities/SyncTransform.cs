using System;
using UnityEngine;


[Serializable]
public enum SyncThread
{
    None,
    FixedUpdate,
    Update,
    LateUpdate,
}
[Serializable]
public enum SyncMode
{
    None,
    SyncRotation,
    Position,
    Both
}

[Serializable]
public class SyncMethod
{
    public SyncMode mode;
    public SyncThread thread;
}
public class SyncTransform : MonoBehaviour
{

    public Transform Target;
    
    [SerializeField] SyncMode mode = SyncMode.None;
    [SerializeField] SyncThread thread = SyncThread.None;

    void SyncPosition()
    {
        if (mode == SyncMode.Position || mode == SyncMode.Both)
            transform.position = Target.position;
    }
    void SyncRotation()
    {
        if (mode == SyncMode.SyncRotation || mode == SyncMode.Both)
            transform.rotation = Target.rotation;
    }

    public void SetSyncMethod(SyncMethod method)
    {
        if(mode != method.mode)
        {
            mode= method.mode;
        }
        if(thread != method.thread)
        {
            thread = method.thread;
        }
    }

    void Start()
    {
        SyncPosition();
        SyncRotation();
    }

    void Update()
    {
        if (thread is not SyncThread.Update) return;

        SyncPosition();
        SyncRotation();
    }

    void FixedUpdate()
    {
        if (thread is not SyncThread.FixedUpdate) return;

        SyncPosition();
        SyncRotation();
    }

    private void LateUpdate()
    {
        if (thread is not SyncThread.LateUpdate) return;

        SyncPosition();
        SyncRotation();
    }



}

using UnityEngine;

public struct EnemyEvent
{
    public EnemyEventType type;
    public Vector3 position;
    public object data;

    public EnemyEvent(EnemyEventType type, Vector3 pos, object data = null)
    {
        this.type = type;
        this.position = pos;
        this.data = data;
    }
}

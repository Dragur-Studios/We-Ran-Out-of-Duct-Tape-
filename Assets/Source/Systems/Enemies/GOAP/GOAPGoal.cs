using UnityEngine;

[System.Serializable]
public class GOAPGoal
{
    public GOAPKey key;
    public GOAPValueType type;

    public bool boolValue;
    public int intValue;
    public float floatValue;
    public Vector3 vector3Value;
    public Transform transformValue; // if you add Transform support

    public object ExpectedValue
    {
        get
        {
            return type switch
            {
                GOAPValueType.Bool => boolValue,
                GOAPValueType.Int => intValue,
                GOAPValueType.Float => floatValue,
                GOAPValueType.Vector3 => vector3Value,
                GOAPValueType.Transform => transformValue,
                _ => null
            };
        }
    }
}

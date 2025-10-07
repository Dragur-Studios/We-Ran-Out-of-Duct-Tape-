using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GOAPValueType { Bool, Int, Float, Vector3, Transform }

[CreateAssetMenu(menuName = "GOAP/Action Set")]
public class GOAPActionSet : ScriptableObject
{
    [Tooltip("List of actions included in this set")]
    public List<GOAPAction> actions = new();

    [Tooltip("Optional goals this set wants to achieve")]
    public List<GOAPGoal> goals = new();

    [System.Serializable]
    public class WorldStateSeed
    {
        public GOAPKey key;
        public GOAPValueType type;

        public bool boolValue;
        public int intValue;
        public float floatValue;
        public Vector3 vector3Value;
        public Transform transformValue;

        public object Value
        {
            get => type switch
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




    [Tooltip("World state keys this set cares about")]
    public List<WorldStateSeed> worldStateProfile = new();

    public List<WorldStateSeed> GetSeeds()
    {
        return worldStateProfile;
    }
}

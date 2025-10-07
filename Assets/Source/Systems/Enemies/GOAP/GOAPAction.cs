using System.Collections.Generic;
using UnityEngine;

public abstract class GOAPAction : ScriptableObject
{
    public string actionName = "Action";
    public float cost = 1f;
    public float duration = 0.5f;

    [HideInInspector] public List<GOAPGoal> preconditions = new List<GOAPGoal>();
    [HideInInspector] public List<GOAPGoal> effects = new List<GOAPGoal>();

    public bool InRange { get; protected set; }
    public GameObject agent { get; private set; }
    public Transform target { get; protected set; }

    public void Initilize(GameObject agentGo)
    {
        agent = agentGo;
        ResetAction();
    }

    public void ResetAction()
    {
        InRange = false;
        target = null;
    }

    public abstract bool CheckPreconditions(GOAPWorldState worldState);
    public abstract bool StartAction(GOAPWorldState worldState);
    public abstract bool PerformAction(float deltaTime, GOAPWorldState worldState);

    public virtual bool RequiresInRange() => false;
    public virtual void SetTarget(Transform t) { target = t; }
    public virtual void OnDrawGizmosSelected() { }
}

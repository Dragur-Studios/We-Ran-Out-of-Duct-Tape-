using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GOAP/ChaseTarget")]
public class ChaseTargetAction : GOAPAction
{
    public float acceptableRange = 1.5f;

    private void OnEnable()
    {
        actionName = "ChaseTarget";
        preconditions = new List<GOAPGoal> { 
            new (){ key = GOAPKey.HasTarget, type=GOAPValueType.Bool, boolValue = true, } 
        };
        effects = new List<GOAPGoal> { 
            new (){ key = GOAPKey.AtTarget, type = GOAPValueType.Bool, boolValue = true } 
        };
    }

    public override bool CheckPreconditions(GOAPWorldState state)
    {
        // Must have a target (Transform or Vector3)
        return state.Has(GOAPKey.Target) && state.Get<object>(GOAPKey.Target, null) != null;
    }

    // ChaseTargetAction.cs (StartAction)
    public override bool StartAction(GOAPWorldState state)
    {
        // Clear AtTarget on restart
        state.Set(GOAPKey.AtTarget, false);

        var t = state.Get<Transform>(GOAPKey.Target, null);
        if (t != null)
        {
            SetTarget(t);
            state.Set(GOAPKey.MoveTarget, t);
            state.Set(GOAPKey.Moving, true);
            return true;
        }

        Vector3 pos = state.Get(GOAPKey.Target, Vector3.zero);
        if (pos != Vector3.zero)
        {
            state.Set(GOAPKey.MoveTarget, pos);
            state.Set(GOAPKey.Moving, true);
            return true;
        }
        return false;
    }


    public override bool PerformAction(float deltaTime, GOAPWorldState state)
    {
        // Wait until driver clears Moving
        if (!state.Get(GOAPKey.Moving, true))
        {
            state.Set(GOAPKey.AtTarget, true);
            return true;
        }
        return false;
    }

    public override bool RequiresInRange() => true;
}


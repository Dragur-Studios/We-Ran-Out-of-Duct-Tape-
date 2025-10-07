using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GOAP/Investigate")]
public class InvestigateAction : GOAPAction
{
    private void OnEnable()
    {
        actionName = "Investigate";
        preconditions = new List<GOAPGoal> { new() { key = GOAPKey.LastHeardTargetPosition, type=GOAPValueType.Vector3} };
        effects = new List<GOAPGoal> { new() { key = GOAPKey.InvestigatingCompleted, type = GOAPValueType.Bool, boolValue = true } };
    }

    public override bool CheckPreconditions(GOAPWorldState state)
        => state.Get(GOAPKey.LastHeardTarget, false);

    public override bool StartAction(GOAPWorldState state)
    {
        var pos = state.Get<Vector3>(GOAPKey.LastHeardTargetPosition, Vector3.zero);
        if (pos == Vector3.zero) return false;

        state.Set(GOAPKey.MoveTarget, pos);
        state.Set(GOAPKey.Moving, true);
        return true;
    }

    public override bool PerformAction(float deltaTime, GOAPWorldState state)
    {
        if (!state.Get(GOAPKey.Moving, true))
        {
            state.Set(GOAPKey.Investigating, false);
            state.Set(GOAPKey.LastHeardTarget, false);
            return true;
        }
        return false;
    }
}

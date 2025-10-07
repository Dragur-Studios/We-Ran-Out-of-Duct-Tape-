using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GOAP/MoveTo")]
public class MoveToAction : GOAPAction
{
    public float acceptableRange = 1.2f;

    private void OnEnable()
    {
        actionName = "MoveTo";
        preconditions = new List<GOAPGoal> { new() { key = GOAPKey.HasFoodTarget, type = GOAPValueType.Bool, boolValue = true } };
        effects = new List<GOAPGoal> { new() { key = GOAPKey.AtFood, type = GOAPValueType.Bool, boolValue = true } };
    }

    public override bool CheckPreconditions(GOAPWorldState state)
        => state.Get(GOAPKey.HasFoodTarget, false) && state.Get<Transform>(GOAPKey.FoodTarget, null) != null;

    public override bool StartAction(GOAPWorldState state)
    {
        var t = state.Get<Transform>(GOAPKey.FoodTarget, null);
        if (t == null) return false;

        SetTarget(t);
        state.Set(GOAPKey.MoveTarget, t);
        state.Set(GOAPKey.Moving, true);
        return true;
    }

    public override bool PerformAction(float deltaTime, GOAPWorldState state)
    {
        if (!state.Get(GOAPKey.Moving, true))
        {
            state.Set(GOAPKey.AtFood, true);
            return true;
        }
        return false;
    }

    public override bool RequiresInRange() => true;
}

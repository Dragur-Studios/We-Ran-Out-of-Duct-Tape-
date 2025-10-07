using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GOAP/FindFood")]
public class FindFoodAction : GOAPAction
{
    public float searchRadius = 20f;
    public LayerMask foodMask;
    public float searchInterval = 0.5f;

    Transform found;
    float timer;

    private void OnEnable()
    {
        actionName = "FindFood";
        preconditions = new List<GOAPGoal>();
        effects = new List<GOAPGoal>
        {
            new GOAPGoal{ key=GOAPKey.HasFoodTarget, type=GOAPValueType.Bool, boolValue=true },
            new GOAPGoal{ key=GOAPKey.KnownFood, type=GOAPValueType.Bool, boolValue=true },
            new GOAPGoal{ key=GOAPKey.FoodTarget, type=GOAPValueType.Transform, transformValue=found }
        };
    }

    public override bool CheckPreconditions(GOAPWorldState state) => true;

    public override bool StartAction(GOAPWorldState state)
    {
        InRange = false;
        timer = 0f;
        found = null;

        var known = state.Get<Transform>(GOAPKey.KnownFood, null);
        if (known != null)
        {
            found = known;
            state.Set(GOAPKey.HasFoodTarget, true);
            state.Set(GOAPKey.FoodTarget, found);
            SetTarget(found);
            return true;
        }
        return true;
    }

    public override bool PerformAction(float deltaTime, GOAPWorldState state)
    {
        if (found != null)
        {
            state.Set(GOAPKey.HasFoodTarget, true);
            state.Set(GOAPKey.FoodTarget, found);
            return true;
        }

        timer -= deltaTime;
        if (timer <= 0f)
        {
            timer = searchInterval;
            Collider[] hits = Physics.OverlapSphere(agent.transform.position, searchRadius, foodMask);
            if (hits.Length > 0)
            {
                float best = float.MaxValue;
                Transform bestT = null;
                foreach (var h in hits)
                {
                    float d = (h.transform.position - agent.transform.position).sqrMagnitude;
                    if (d < best) { best = d; bestT = h.transform; }
                }

                if (bestT != null)
                {
                    found = bestT;
                    state.Set(GOAPKey.HasFoodTarget, true);
                    state.Set(GOAPKey.KnownFood, found);
                    state.Set(GOAPKey.FoodTarget, found);
                    SetTarget(found);
                    return true;
                }
            }
        }
        return false;
    }

    public override bool RequiresInRange() => false;
}

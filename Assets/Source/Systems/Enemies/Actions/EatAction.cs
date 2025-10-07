using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GOAP/Eat")]
public class EatAction : GOAPAction
{
    public float eatDuration = 2.0f;
    float timer;
    Transform food;

    private void OnEnable()
    {
        actionName = "Eat";
        preconditions = new List<GOAPGoal> { 
            new() { key = GOAPKey.AtFood, type=GOAPValueType.Bool, boolValue=true } 
        };
        effects = new List<GOAPGoal> { 
            new() { key = GOAPKey.Fed, type = GOAPValueType.Bool, boolValue = true } 
        };
        duration = eatDuration;
    }

    public override bool CheckPreconditions(GOAPWorldState state)
        => state.Get(GOAPKey.AtFood, false);

    public override bool StartAction(GOAPWorldState state)
    {
        timer = eatDuration;
        food = state.Get<Transform>(GOAPKey.FoodTarget, null);
        if (food != null) SetTarget(food);
        return food != null;
    }

    public override bool PerformAction(float deltaTime, GOAPWorldState state)
    {
        timer -= deltaTime;
        if (timer <= 0f)
        {
            state.Set(GOAPKey.Fed, true);
            state.Set(GOAPKey.HasFoodTarget, false);
            state.Set(GOAPKey.AtFood, false);
            state.Set(GOAPKey.FoodTarget, null);
            state.Set(GOAPKey.KnownFood, null);

            if (food != null)
                GameObject.Destroy(food.gameObject);

            return true;
        }
        return false;
    }

    public override bool RequiresInRange() => true;
}

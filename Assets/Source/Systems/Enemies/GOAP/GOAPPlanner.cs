using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class GOAPPlanner
{
    public static List<GOAPAction> Plan(
        GameObject agent,
        List<GOAPAction> availableActions,
        GOAPWorldState worldState,
        List<GOAPGoal> goalEffects,
        int maxDepth = 5)
    {
        // Reset actions
        foreach (var a in availableActions) a.ResetAction();

        var usable = availableActions.Where(a => a.CheckPreconditions(worldState)).ToList();
        var plan = new List<GOAPAction>();

        bool DFS(GOAPWorldState state, List<GOAPAction> currentPlan, int depth)
        {
            if (depth > maxDepth) return false;

            // Check if goal satisfied
            bool goalMet = true;
            foreach (var g in goalEffects)
            {
                if (!state.Has(g.key) || !state.Get(g.key, false))
                {
                    goalMet = false;
                    break;
                }
            }
            if (goalMet)
            {
                plan = new List<GOAPAction>(currentPlan);
                return true;
            }

            // Try each usable action
            foreach (var action in usable)
            {
                if (currentPlan.Contains(action)) continue;
                if (!action.CheckPreconditions(state)) continue;

                // simulate effects
                var newState = state.Clone();
                foreach (var eff in action.effects)
                    newState.Set(eff.key, true);

                currentPlan.Add(action);
                if (DFS(newState, currentPlan, depth + 1)) return true;
                currentPlan.RemoveAt(currentPlan.Count - 1);
            }

            return false;
        }

        var startPlan = new List<GOAPAction>();
        if (DFS(worldState.Clone(), startPlan, 0))
        {
            foreach (var a in plan) a.Initilize(agent);
            return plan;
        }

        return null;
    }
}

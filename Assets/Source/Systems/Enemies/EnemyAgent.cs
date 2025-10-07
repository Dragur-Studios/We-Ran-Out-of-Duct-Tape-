using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static GOAPActionSet;


[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAgent : MonoBehaviour
{
    public List<GOAPActionSet> actionSets = new List<GOAPActionSet>();


    List<GOAPGoal> _goals = new() { };
    List<GOAPAction> _actions = new();
    public List<GOAPAction> AvailableAtions { get { return _actions; } }

    [Header("Planning")]
    public float replanDelay = 0.5f;
    public float periodicReplanInterval = 0.15f;

    GOAPWorldState worldState = new();
    List<GOAPAction> currentPlan;
    GOAPAction currentAction;
    int planIndex;
    Transform lastFoodTarget = null;

    void Start()
    {
        // clone actions
        var instanced = new List<GOAPAction>();
        
        // build the actions from the action sets..
        var actions = new List<GOAPAction>();
        var goals = new List<GOAPGoal>();

        // merge all action sets.

        foreach (var set in actionSets)
        {
            // must be non null... 
            if (set == null) continue;

            foreach(var a in set.actions)
            {
                // must be non null..
                // must be unique..
                if(a != null && !actions.Contains(a))
                {
                    actions.Add(a);
                }
            }
            // build the list of goals from each of the action sets..
            foreach(var g in set.goals)
            {
                // must be non empty non null..
                // must be unique
                if (!goals.Contains(g))
                {
                    goals.Add(g);
                }
            }

            // seed world state
            foreach (var seed in set.worldStateProfile)
            {
                worldState.Set(seed.key, seed.Value);
            }


        }

        // finally clone the per insatance version of this action list.
        // move it over to the temporary instanced buffer.
        foreach (var a in actions)
        {
            var clone = Instantiate(a);
            clone.Initilize(gameObject);
            instanced.Add(clone);
        }

        _actions = instanced;
        _goals = goals;

        //// seed world state
        //worldState.Set(GOAPKeys.Fed, false);
        //worldState.Set(GOAPKeys.HasFoodTarget, false);
        //worldState.Set(GOAPKeys.AtFood, false);
        //worldState.Set(GOAPKeys.FoodTarget, null);

        foreach (var a in _actions) a.Initilize(gameObject);
        StartCoroutine(PlannerLoop());
    }

    static object ParseSeedValue(string raw)
    {
        if (string.IsNullOrEmpty(raw)) return null;
        if (string.Equals(raw, "null", System.StringComparison.OrdinalIgnoreCase)) return null;

        if (bool.TryParse(raw, out var b)) return b;
        if (int.TryParse(raw, out var i)) return i;
        if (float.TryParse(raw, out var f)) return f;

        // Vector3 as "x,y,z" (spaces tolerated)
        var parts = raw.Split(',');
        if (parts.Length == 3 &&
            float.TryParse(parts[0].Trim(), out var x) &&
            float.TryParse(parts[1].Trim(), out var y) &&
            float.TryParse(parts[2].Trim(), out var z))
        {
            return new Vector3(x, y, z);
        }

        // Fallback: keep as string for debugging keys that are labels
        return raw;
    }

    // EnemyAgent.cs (inside HandleEvent)
    public void HandleEvent(EnemyEvent e)
    {
        switch (e.type)
        {
            case EnemyEventType.HeardSound:
                worldState.Set(GOAPKey.LastHeardTargetPosition, e.position);
                worldState.Set(GOAPKey.LastHeardTarget, true);
                worldState.Set(GOAPKey.Investigating, true);
                RequestReplan(ReplanMode.AfterDelay);
                break;

            case EnemyEventType.SawSomething:
                worldState.Set(GOAPKey.LastSeenTargetPosition, e.position);
                worldState.Set(GOAPKey.HasSeenTarget, true);

                if (e.data is Transform t)
                {
                    worldState.Set(GOAPKey.Target, t);
                    worldState.Set(GOAPKey.HasTarget, true);
                    worldState.Set(GOAPKey.TargetAlive, true);

                    worldState.Set(GOAPKey.AtTarget, false);
                }

                RequestReplan(ReplanMode.ForceImmediate);
                break;

            case EnemyEventType.TookDamage:
                worldState.Set(GOAPKey.UnderAttack, true);
                RequestReplan(ReplanMode.ForceImmediate);
                break;
        }
    }


    public GOAPWorldState GetWorldState() => worldState;
    public enum ReplanMode
    {
        ForceImmediate,
        IfNeeded,
        AfterDelay,
        Opportunistic
    }

    // --- Public API ---
    public void RequestReplan(ReplanMode mode)
    {
        switch (mode)
        {
            case ReplanMode.ForceImmediate:
                StopAllCoroutines(); // stop periodic loop too
                StartCoroutine(ExecuteReplan(force: true));
                StartCoroutine(PlannerLoop()); // restart periodic loop
                break;

            case ReplanMode.IfNeeded:
                StartCoroutine(ExecuteReplan(force: false));
                break;

            case ReplanMode.AfterDelay:
                StartCoroutine(ExecuteReplanAfterDelay());
                break;

            case ReplanMode.Opportunistic:
                if (WorldStateChangedSignificantly())
                    StartCoroutine(ExecuteReplan(force: false));
                break;
        }
    }

    // --- Core replanning coroutines ---
    IEnumerator PlannerLoop()
    {
        while (true)
        {
            yield return ExecuteReplan(force: false);
            yield return new WaitForSecondsRealtime(periodicReplanInterval);
        }
    }

    IEnumerator ExecuteReplanAfterDelay()
    {
        yield return new WaitForSeconds(replanDelay);
        yield return ExecuteReplan(force: false);
    }

    IEnumerator ExecuteReplan(bool force)
    {
        if (force || currentPlan == null || !GoalSatisfied())
        {
            var plan = GOAPPlanner.Plan(this.gameObject, _actions, worldState, _goals, maxDepth: 6);
            if (plan != null && plan.Count > 0)
            {
                currentPlan = plan;
                planIndex = 0;
                StartNextAction();
            }
            else
            {
                // fallback: wander
                var wander = _actions.Find(a => a is WanderAction);
                if (wander != null)
                {
                    currentPlan = new List<GOAPAction> { wander };
                    planIndex = 0;
                    StartNextAction();
                }
            }
        }
        yield return null;
    }

    // --- Update loop ---
    void Update()
    {
        if (currentAction != null)
        {
            bool finished = currentAction.PerformAction(Time.deltaTime, worldState);
            if (finished)
            {
                planIndex++;
                if (planIndex >= currentPlan.Count)
                {
                    currentPlan = null;
                    currentAction = null;
                }
                else
                {
                    StartNextAction();
                }
            }
        }
    }

    // --- Helpers ---

    void StartNextAction()
    {
        if (currentPlan == null || planIndex >= currentPlan.Count) return;
        currentAction = currentPlan[planIndex];
        currentAction.Initilize(this.gameObject);

        // Prefer explicit action target from world state
        var foodT = worldState.Get<Transform>(GOAPKey.FoodTarget, null);
        var chaseT = worldState.Get<Transform>(GOAPKey.Target, null);

        if (foodT != null) currentAction.SetTarget(foodT);
        else if (chaseT != null) currentAction.SetTarget(chaseT);

        // If an action fails to start, drop plan to avoid deadlock
        if (!currentAction.StartAction(worldState))
        {
            currentPlan = null;
            currentAction = null;
        }
    }
    bool GoalSatisfied()
    {
        foreach (var g in _goals)
        {
            if (!worldState.Has(g.key)) return false;
            var current = worldState.AsDictionary()[g.key];
            if (!Equals(current, g.ExpectedValue)) return false;
        }
        return true;
    }


    bool WorldStateChangedSignificantly()
    {
        var t = worldState.Get<Transform>(GOAPKey.FoodTarget, null);
        if (t != lastFoodTarget)
        {
            lastFoodTarget = t;
            return true;
        }
        return false;
    }

    public GOAPAction GetCurrentAction() => currentAction;
}

public class GOAPWorldState
{
    private readonly Dictionary<GOAPKey, object> state = new();

    public T Get<T>(GOAPKey key, T defaultValue = default)
    {
        if (state.TryGetValue(key, out var val) && val is T t) return t;
        return defaultValue;
    }

    public void Set(GOAPKey key, object value) => state[key] = value;
    public bool Has(GOAPKey key) => state.ContainsKey(key);
    public Dictionary<GOAPKey, object> AsDictionary() => new(state);

    public GOAPWorldState Clone()
    {
        var clone = new GOAPWorldState();
        foreach (var kvp in state)
            clone.Set(kvp.Key, kvp.Value);
        return clone;
    }
}

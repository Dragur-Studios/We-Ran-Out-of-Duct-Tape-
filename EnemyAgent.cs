using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAgent : MonoBehaviour
{
    public List<GOAPAction> availableActions = new();
    public List<string> primaryGoal = new() { GOAPKeys.Fed };

    [Header("Planning")]
    public float replanDelay = 0.5f;
    public float periodicReplanInterval = 0.15f;

    EnemyWorldState worldState = new();
    List<GOAPAction> currentPlan;
    GOAPAction currentAction;
    int planIndex;

    void Start()
    {
        // clone actions
        var instanced = new List<GOAPAction>();
        foreach (var a in availableActions)
        {
            var clone = Instantiate(a);
            clone.Initilize(gameObject);
            instanced.Add(clone);
        }
        availableActions = instanced;

        // seed world state
        worldState.Set(GOAPKeys.Fed, false);
        worldState.Set(GOAPKeys.HasFoodTarget, false);
        worldState.Set(GOAPKeys.AtFood, false);
        worldState.Set(GOAPKeys.FoodTarget, null);

        foreach (var a in availableActions) a.Initilize(gameObject);
        StartCoroutine(PlannerLoop());
    }

    public void HandleEvent(EnemyEvent e)
    {
        switch (e.type)
        {
            case EnemyEventType.HeardSound:
                worldState.Set("LastHeardPlayerPos", e.position);
                worldState.Set("HasHeardPlayer", true);
                worldState.Set("Investigating", true);
                RequestReplan(ReplanMode.AfterDelay);
                break;

            case EnemyEventType.SawPlayer:
                worldState.Set("LastSeenPlayerPos", e.position);
                worldState.Set("HasSeenPlayer", true);
                RequestReplan(ReplanMode.ForceImmediate);
                break;

            case EnemyEventType.TookDamage:
                worldState.Set("UnderAttack", true);
                RequestReplan(ReplanMode.ForceImmediate);
                break;
        }
    }

    public EnemyWorldState GetWorldState() => worldState;
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
            UpdateSenses();

            var plan = GOAPPlanner.Plan(this.gameObject, availableActions, worldState, primaryGoal, maxDepth: 6);
            if (plan != null && plan.Count > 0)
            {
                currentPlan = plan;
                planIndex = 0;
                StartNextAction();
            }
            else
            {
                // fallback: wander
                var wander = availableActions.Find(a => a is WanderAction);
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

        if (worldState.TryGetValue(GOAPKeys.FoodTarget, out var o) && o is Transform t)
            currentAction.SetTarget(t);

        if (!currentAction.StartAction(worldState))
        {
            currentPlan = null;
            currentAction = null;
        }
    }

    bool GoalSatisfied()
    {
        foreach (var g in primaryGoal)
        {
            if (!worldState.ContainsKey(g) || !(worldState[g] is bool b && b))
                return false;
        }
        return true;
    }

    void UpdateSenses()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, visionRange, foodMask);
        if (hits.Length > 0)
        {
            float best = float.MaxValue;
            Transform bestT = null;
            foreach (var h in hits)
            {
                var d = (h.transform.position - transform.position).sqrMagnitude;
                if (d < best) { best = d; bestT = h.transform; }
            }
            if (bestT != null)
            {
                worldState[GOAPKeys.KnownFood] = bestT;
                worldState[GOAPKeys.HasFoodTarget] = true;
                worldState[GOAPKeys.FoodTarget] = bestT;
            }
        }
    }

    bool WorldStateChangedSignificantly()
    {
        if (worldState.TryGetValue(GOAPKeys.FoodTarget, out var t) && t != lastFoodTarget)
        {
            lastFoodTarget = t;
            return true;
        }
        return false;
    }

    // --- Exposed for other components ---
    public Dictionary<string, object> GetWorldState() => worldState;
    public GOAPAction GetCurrentAction() => currentAction;
}

public class EnemyWorldState
{
    private readonly Dictionary<string, object> state = new();

    public T Get<T>(string key, T defaultValue = default)
    {
        if (state.TryGetValue(key, out var val) && val is T t) return t;
        return defaultValue;
    }

    public void Set(string key, object value) => state[key] = value;
    public bool Has(string key) => state.ContainsKey(key);
    public Dictionary<string, object> AsDictionary() => state;
}
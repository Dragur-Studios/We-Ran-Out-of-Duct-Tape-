using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GOAP/AttackTarget")]
public class AttackTargetAction : GOAPAction
{
    public float attackDuration = 1.0f;
    float timer;

    private void OnEnable()
    {
        actionName = "AttackTarget";
        preconditions = new List<GOAPGoal> { new() { key = GOAPKey.AtTarget, type = GOAPValueType.Bool, boolValue = true } };
        effects = new List<GOAPGoal> { new() { key = GOAPKey.TargetDamaged, type = GOAPValueType.Bool, boolValue = true } };
    }

    public override bool CheckPreconditions(GOAPWorldState state)
        => state.Get(GOAPKey.AtTarget, false) && state.Get(GOAPKey.TargetAlive, true);

    public override bool StartAction(GOAPWorldState state)
    {
        timer = attackDuration;

        var anim = agent.GetComponentInChildren<Animator>();
        anim?.SetTrigger("Attack");

        return true;
    }

    public override bool PerformAction(float deltaTime, GOAPWorldState state)
    {
        timer -= deltaTime;
        if (timer <= 0f)
        {
            state.Set(GOAPKey.TargetDamaged, true);

            var player = state.Get<Transform>(GOAPKey.Target, null);
            if (player == null || player.GetComponent<Player>()?.IsDead() == true)
            {
                state.Set(GOAPKey.TargetAlive, false);
                state.Set(GOAPKey.TargetDead, true);
            }

            return true; 
        }
        return false;
    }

    public override bool RequiresInRange() => true;
}

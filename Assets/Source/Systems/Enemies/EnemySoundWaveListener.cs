using UnityEngine;

[RequireComponent(typeof(EnemyAgent))]
public class EnemySoundWaveListener : SoundWaveListener
{
    EnemyAgent agent;
    Enemy enemy;

    void Awake()
    {
        agent = GetComponent<EnemyAgent>();
        enemy = GetComponent<Enemy>();
    }

    protected override void ReactToSoundHeard(Vector3 sourcePos, string tag)
    {
        if (enemy != null && enemy.IsDead()) return;

        // Forward the event to the agent
        //agent?.AlertToSound(sourcePos);
        agent?.HandleEvent(new EnemyEvent(EnemyEventType.HeardSound, sourcePos));
    }

    void OnEnemyDied()
    {
        // Optional: unregister from SoundWaveManager or disable this component
        enabled = false;
        SoundWaveManager.RemoveListener(this);
    }
}

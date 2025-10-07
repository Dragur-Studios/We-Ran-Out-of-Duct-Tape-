using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(EnemyAgent))]
public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float maxHP = 100f;
    [SerializeField] float moveSpeed = 2.3f;
    [SerializeField] int damagePotential = 40;

    [Header("Runtime")]
    [SerializeField] bool isPlayerHoveringMe = false;

    // Cached components
    EnemyAgent agent;
    NavMeshAgent navAgent;
    Collider mainCollider;
    Animator animator;

    float hp;
    bool isDead = false;

    void Awake()
    {
        hp = maxHP;
        agent = GetComponent<EnemyAgent>();
        navAgent = GetComponent<NavMeshAgent>() ?? GetComponentInChildren<NavMeshAgent>();
        mainCollider = GetComponent<Collider>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (hp <= 0f) Die();
    }

    public void DealDamage(float value)
    {
        if (isDead) return;
        hp -= value;
        if (hp <= 0f) Die();
    }

    public int GetDamage() => damagePotential;
    public float GetHP() => hp;
    public bool IsDead() => isDead;

    public void SetIsPlayerLookingAtMe(bool v)
    {
        isPlayerHoveringMe = v;
        SetLayer(isPlayerHoveringMe ? "Enemy - Outline" : "Enemy");
    }

    void Die()
    {
        if (isDead) return;
        SetIsPlayerLookingAtMe(false);
        isDead = true;

        animator?.SetTrigger("Death");
        if (mainCollider != null) mainCollider.enabled = false;
        if (agent != null) agent.enabled = false;
        if (navAgent != null) navAgent.enabled = false;

        // Let other components (like EnemySoundWaveListener) clean themselves up
        SendMessage("OnEnemyDied", SendMessageOptions.DontRequireReceiver);
    }

    void SetLayer(string layerName)
    {
        int layerIndex = LayerMask.NameToLayer(layerName);
        if (layerIndex == -1)
        {
            Debug.LogWarning($"Layer \"{layerName}\" does not exist!");
            return;
        }

        gameObject.layer = layerIndex;
        foreach (var r in GetComponentsInChildren<Renderer>(true))
            r.gameObject.layer = layerIndex;
    }
}

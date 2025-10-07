using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class EnemyAnimationResolver : MonoBehaviour
{
    Enemy enemy;
    Animator anim;
    NavMeshAgent agent;
    int idle = 0;

    private Vector3 lastPos;
    private float velocityFactor;

    void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
        agent = enemy.GetComponent<NavMeshAgent>();
        
        anim = GetComponent<Animator>();
        anim.speed = Random.Range(0.9f, 1.1f); 

        idle = Random.Range(1, 4);
        anim.SetInteger("Idle", idle);

        CloseDamageCollider();
    }
  
    void Update()
    {
        var anim = GetComponentInChildren<Animator>();


        var speed = agent.speed;

        if (speed < 0.1f)
            velocityFactor = 0f;
        else if (speed < 1.5f)
            velocityFactor = 0.5f;
        else
            velocityFactor = 1.0f;

        anim.SetFloat("Velocity", velocityFactor);

    }
    [SerializeField] GameObject damageCollider;
    public void OpenDamageCollider()
    {
        damageCollider.SetActive(true);
    }

    public void CloseDamageCollider()
    {
        damageCollider.SetActive(false);
    }
}

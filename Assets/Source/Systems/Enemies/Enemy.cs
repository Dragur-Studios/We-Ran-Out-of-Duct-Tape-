using UnityEngine;
using UnityEngine.AI;

public class Enemy : SoundWaveListener
{
    float HP = 100;

    EnemyBehaviorResolver behaviorResolver;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        behaviorResolver = GetComponent<EnemyBehaviorResolver>();        
    }


    protected override void ReactToSoundHeard(Vector3 sourcePos, string tag)
    {
        behaviorResolver.AlertToPlayer(sourcePos);
    }

    // Update is called once per frame

    public bool isPlayerHoveringMe = false;

    public void SetLayer(string layerName)
    {
        int layerIndex = LayerMask.NameToLayer(layerName);
        if (layerIndex == -1)
        {
            Debug.LogWarning($"Layer \"{layerName}\" does not exist!");
            return;
        }

        gameObject.layer = layerIndex;

        var skinnedMeshes = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var mesh in skinnedMeshes)
        {
            mesh.gameObject.layer = layerIndex;
        }
    }


    void Update()
    {

        SetLayer(isPlayerHoveringMe ? "Enemy - Outline" : "Enemy");


        if (HP <= 0)
        {
            var anim = GetComponentInChildren<Animator>();
            anim.SetTrigger("Death");

            var col = GetComponent<Collider>();
            col.enabled = false;

            behaviorResolver.enabled = false;

            var nma = col.GetComponent<NavMeshAgent>();
            nma.enabled = false;

            SoundWaveManager.RemoveListener(this);

        }
    }

    private void LateUpdate()
    {
        isPlayerHoveringMe = false;
    }

    public void DealDamage(float value)
    {
        HP -= value;
    }

    [SerializeField] int DamagePotential = 40;

    internal int GetDamage()
    {
        return DamagePotential;
    }

    public  void SetIsPlayerLookingAtMe(bool v)
    {
        isPlayerHoveringMe = v;
    }
}

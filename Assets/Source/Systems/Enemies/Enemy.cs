using System;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    float HP = 100;

    EnemyBehaviorResolver behaviorResolver;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        behaviorResolver = GetComponent<EnemyBehaviorResolver>();        
    }

    // Update is called once per frame
    void Update()
    {
        if(HP <= 0)
        {
            var anim = GetComponentInChildren<Animator>();
            anim.SetTrigger("Death");

            var col = GetComponent<Collider>();
            col.enabled = false;

            behaviorResolver.enabled = false;

            var nma = col.GetComponent<NavMeshAgent>();
            nma.enabled = false;


        }
    }

    public void DealDamage(float value)
    {
        HP -= value;
    }
}

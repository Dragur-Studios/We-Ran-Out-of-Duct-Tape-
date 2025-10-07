using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class PlayerCombatHandler : MonoBehaviour
{
    public Weapon equippedWeapon;

    PlayerRigOrchastrator rigs;
    GameInputReciever inputs;

    bool isLock = false;

    [SerializeField] float lookRange = 20f;       
    [SerializeField] LayerMask enemyMask;

    Player player;

    public void Lock()
    {
        isLock = true;
    }
    public void Unlock()
    {
        isLock = false;
    }
    private void Start()
    {
        player = GetComponent<Player>();

        rigs = GetComponentInChildren<PlayerRigOrchastrator>();
        inputs = player.Input;
    }

    private void Update()
    {
        if (hover != null)
        {
            hover.SetIsPlayerLookingAtMe(false);
            hover = null;
        }

        if (isLock) return;
        
        if (inputs.Focus)
        {
            CheckEnemyLook();

            equippedWeapon.EnableLaserPointer();
        }
        else
        {
            equippedWeapon.DisableLaserPointer();
        }

        if (inputs.Fire)
        {
            rigs.TryFire(equippedWeapon.Fire);
        }

      
    }

    Enemy hover = null;
    
    private void CheckEnemyLook()
    {
        
        Ray ray = new Ray(transform.position + Vector3.up * 1.5f, transform.forward);
        if (Physics.SphereCast(ray, 0.3f, out RaycastHit hit, lookRange, enemyMask))
        {
            var enemy = hit.collider.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                enemy.SetIsPlayerLookingAtMe(true);
                hover = enemy;
            }
        }
       
        
        
    }
}

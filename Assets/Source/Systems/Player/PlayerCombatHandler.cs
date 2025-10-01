using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class PlayerCombatHandler : MonoBehaviour
{
    public Weapon equippedWeapon;

    PlayerRigOrchastrator rigs;
    PlayerInputReciever inputs;

    bool isLock = false;

    [SerializeField] float lookRange = 20f;       // how far forward to check
    [SerializeField] LayerMask enemyMask;         // assign "Enemy" layer in inspector

    internal void Lock()
    {
        isLock = true;
    }

    private void Start()
    {
        rigs = GetComponentInChildren<PlayerRigOrchastrator>();
        inputs = GetComponent<PlayerInputReciever>();
    }

    private void Update()
    {
        if (isLock) return;

        if (inputs.Focus)
        {
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

        CheckEnemyLook();
    }

    private void CheckEnemyLook()
    {
        // Cast a ray from the player’s position forward
        Ray ray = new Ray(transform.position + Vector3.up * 1.5f, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, lookRange, enemyMask))
        {
            // If we hit an enemy, set its flag
            Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                // You’ll need to add this property/method to Enemy
                enemy.SetIsPlayerLookingAtMe(true);
            }
        }
        else
        {
            // Optionally, clear the flag on all enemies in range
            // (depends on your design — you might want enemies to reset themselves)
        }
    }
}

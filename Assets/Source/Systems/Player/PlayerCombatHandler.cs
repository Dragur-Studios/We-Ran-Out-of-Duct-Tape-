using System;
using System.Collections;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class PlayerCombatHandler : MonoBehaviour
{
    public Weapon equippedWeapon;

    PlayerRigOrchastrator rigs;
    PlayerInputReciever inputs;


    bool isLock = false;

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

        if (inputs.Fire)
        {
            rigs.TryFire(equippedWeapon.Fire);
            
        }
    }


}

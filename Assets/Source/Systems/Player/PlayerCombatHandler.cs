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

    private void Start()
    {
        rigs = GetComponentInChildren<PlayerRigOrchastrator>();
        inputs = GetComponent<PlayerInputReciever>();
    }

    private void Update()
    {
        if (inputs.Fire)
        {
            if (rigs.TryFire())
            {
                equippedWeapon.Fire();
            }
        }
    }


}

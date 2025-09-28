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

    private void Start()
    {
        rigs = GetComponentInChildren<PlayerRigOrchastrator>();
    }

    private void Update()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            if (rigs.TryFire())
            {
                equippedWeapon.Fire();
            }
        }
    }


}

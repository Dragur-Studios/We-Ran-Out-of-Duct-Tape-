using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

[Serializable]
public class RigTarget
{
    public Rig Rig;
    public Transform Target;
    [Range(0, 1)] public float BlendWeight = 1.0f;

}

[Serializable]
public class HandIKTarget
{
    // HAND
    public RigTarget Root;
    // FINGERS
    public RigTarget Thumb;
    public RigTarget Index;
    public RigTarget Outer;


    public void Update(HandSocket sk, float weight)
    {

        // root socket.
        Root.Target.position = sk.RootSocket.position;
        Root.Target.rotation = sk.RootSocket.rotation;
        Root.Rig.weight = weight;

        // fingers..
        Thumb.Target.position = sk.ThumbSocket.position;
        Thumb.Target.rotation = sk.ThumbSocket.rotation;
        Thumb.Rig.weight = Mathf.Lerp(0, Thumb.BlendWeight, weight);

        Index.Target.position = sk.IndexSocket.position;
        Index.Target.rotation = sk.IndexSocket.rotation;
        Index.Rig.weight = Mathf.Lerp(0, Thumb.BlendWeight, weight);

        Outer.Target.position = sk.OuterSocket.position;
        Outer.Target.rotation = sk.OuterSocket.rotation;
        Outer.Rig.weight = Mathf.Lerp(0, Thumb.BlendWeight, weight);
    }
}

public class PlayerRigOrchastrator : MonoBehaviour
{
    PlayerCombatHandler combat;
    GameInputReciever inputs;
    Player player;

    Weapon equippedWeapon;
    
    [Header("Hand IK")]
    [SerializeField] HandIKTarget leftHand;
    [SerializeField] HandIKTarget rightHand;

    [SerializeField] float handIKTransitionSpeed = 10.0f;


    [Header("State Rigs")]
    [SerializeField] Rig IdleRig;
    [SerializeField] Rig AimRig;
    [SerializeField] Rig RecoilRig;
    [SerializeField] AnimationCurve transitionCurve;

    private void Start()
    {
        player = GetComponentInParent<Player>();
        combat = player.Combat;
        inputs = player.Input;

        RecoilRig.weight = 0;
        AimRig.weight = 0;
        IdleRig.weight = 1;
    }

    float weights_HandIK = 0;

    void HandleWeaponIK()
    {
        if (combat.equippedWeapon != null)
        {
            if (equippedWeapon != combat.equippedWeapon)
            {
                equippedWeapon = combat.equippedWeapon;
            }
        }


        bool target = equippedWeapon != null;
        float value = target ? 1.0f : 0.0f;
        float t = handIKTransitionSpeed * Time.deltaTime;
        weights_HandIK = Mathf.Lerp(weights_HandIK, value, t);

        // update IK based on positions... 

        leftHand.Update(equippedWeapon.LeftHandSocket, weights_HandIK);
        rightHand.Update(equippedWeapon.RightHandSocket, weights_HandIK);
    }

    void HandleProceduralAiming()
    {
        bool isAiming = inputs.Focus;

        float target = isAiming ? 1.0f : 0.0f;
        float speed = 10.0f * Time.deltaTime;

        IdleRig.weight = Mathf.Lerp(IdleRig.weight, 1.0f - target, speed);

        AimRig.weight = Mathf.Lerp(AimRig.weight, target, speed);


    }

    private void Update()
    {
        HandleProceduralAiming(); // always run aiming
        HandleWeaponIK();

        if (!canFire)
        {
            fireDelayTimer -= Time.deltaTime;
            if (fireDelayTimer <= 0)
                canFire = true;
        }
    }




    public bool Firing = false;

    bool canFire = true;
    float fireDelayTimer = 0.0f;


    Coroutine fireAnim;

    public bool TryFire(Action cb)
    {
        if (canFire == false)
            return false;
        
        if (!inputs.Focus) 
            return false;


        cb?.Invoke();

        fireDelayTimer = 60 / equippedWeapon.FireRate;
        canFire = false;
        // HANDLE FIRE..

        if (fireAnim != null)
        {
            StopCoroutine(fireAnim);
        }

        fireAnim = StartCoroutine(HandleFireAnimation(fireDelayTimer));
        return true;
    }

    IEnumerator HandleFireAnimation(float maxTime)
    {
        if (!inputs.Focus)
            yield break;

        float time = 0f;
        Firing = true;

        while (time < maxTime)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / maxTime);

            float weight = transitionCurve.Evaluate(t);

            // Aim fades out as recoil fades in
            AimRig.weight = 1f - weight;
            RecoilRig.weight = weight;

            yield return null;
        }

        // Ensure reset
        AimRig.weight = 1f;
        RecoilRig.weight = 0f;
        Firing = false;
    }


}

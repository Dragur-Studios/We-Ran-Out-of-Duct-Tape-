using System;
using UnityEngine;
public class Player : MonoBehaviour
{

    public float HP = 100.0f;

    public Action<bool> OnCrouch;
    public Action<bool> OnFocus;
    public Action<bool> OnSprint;


    PlayerInventory inventory;
    PlayerMovementResolver movement;
    PlayerAnimationResolver anim;
    PlayerCameraHandler cam;
    PlayerInteractionHandler interaction;
    PlayerInputReciever inputs;
    PlayerCombatHandler combat;
    

    public PlayerInventory Inventory { get => inventory; }
    public PlayerMovementResolver Movement { get => movement; } 
    public PlayerAnimationResolver Animator { get => anim; }
    public PlayerCameraHandler Camera { get => cam; }
    public PlayerInteractionHandler Interaction { get => interaction; } 
    public PlayerInputReciever Input { get => inputs; }
    public PlayerCombatHandler Combat { get => combat; }

    public void SetCrouch(bool crouching) => OnCrouch?.Invoke(crouching);
    public void SetSprint(bool sprinting) => OnSprint?.Invoke(sprinting);
    public void SetFocus(bool focusing) => OnFocus?.Invoke(focusing);

    public void PickUp(IItem item)
    {
        Destroy(item.gameObject);
        inventory.Insert(item);
    }

    private void Update()
    {

    }


    internal void Initilize()
    {
        inventory = GetComponent<PlayerInventory>() ?? gameObject.AddComponent<PlayerInventory>();
        movement = GetComponent<PlayerMovementResolver>() ?? gameObject.AddComponent<PlayerMovementResolver>();
        anim = GetComponentInChildren<PlayerAnimationResolver>();
        combat = GetComponent<PlayerCombatHandler>();

        cam = GetComponent<PlayerCameraHandler>() ?? gameObject.AddComponent<PlayerCameraHandler>();
        cam.SetPlayer(this);

        interaction = GetComponent<PlayerInteractionHandler>() ?? gameObject.AddComponent<PlayerInteractionHandler>();
        inputs = GetComponent<PlayerInputReciever>() ?? gameObject.AddComponent<PlayerInputReciever>();

    }

    internal void TakeDamage(int v)
    {
        HP -= v;

        if (HP <= 0)
        {
            movement.Lock();
            cam.Lock();
            combat.Lock();

            anim.GetComponent<Animator>().CrossFade("Death", 0.1f);
            GameManager.Instance.PlayerDied();
            

            return;
        }
    }
}

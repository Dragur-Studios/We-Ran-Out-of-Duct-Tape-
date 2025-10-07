using System;
using UnityEngine;
using UnityEngine.InputSystem; // For New Input System polling

public class PlayerCameraHandler : MonoBehaviour
{
    [SerializeField] private Transform cinemachineAnchor; // Pivot for the camera
    [SerializeField] private float sensitivityX = 150f;
    [SerializeField] private float sensitivityY = 100f;
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 70f;
    [SerializeField] private float transitionSpeed = 5.0f;

    private float yaw;   // Horizontal rotation
    private float pitch; // Vertical rotation

    [SerializeField] Animator camAnimator;

    bool isFocusing = false;
    bool isCrouching = false;


    Player player;
    GameInputReciever inputs;

  

    void SubscribeCameraEvents()
    {
        player.OnFocus += SetFocus;
        player.OnCrouch += SetCrouch;
    }

    void UnsubscribeCameraEvents()
    {
        player.OnFocus -= SetFocus;
        player.OnCrouch -= SetCrouch;
    }


    public void SetFocus(bool value)
    {
        camAnimator.SetBool("Focus", value);
        isFocusing = value;
    }
    public void SetCrouch(bool value)
    {
        camAnimator.SetBool("isCrouching", value);
        isCrouching = value;
    }

    private void Start()
    {
        player = GetComponent<Player>();
        inputs = player.Input;
    }

    float sk_posT = 0;

    void LateUpdate()
    {
        if (isLock) return;

        // --- 1. Read input ---
        var rotate = inputs.CameraRotation;
        // --- 2. Apply sensitivity & accumulate rotation ---
        // we can only rotate.. when NOT aiming down sight.
        
        yaw += rotate * sensitivityX * Time.deltaTime;
        
        //pitch -= lookInput.y * sensitivityY * Time.deltaTime;
        //pitch = Mathf.Clamp(pitch, minPitch, maxPitch);  

        // --- 4. Apply rotation to anchor ---
        cinemachineAnchor.rotation = Quaternion.Euler(pitch, yaw, 0f);

        // the tracking target. needs to lerp between the two current sockets.. 
        float dir = isCrouching ? 1.0f : -1.0f;

        sk_posT += dir * transitionSpeed * Time.deltaTime;
        sk_posT = Mathf.Clamp01(sk_posT);

    }


    public void SetAnimator(Animator anim)
    {
        UnsubscribeCameraEvents();
        camAnimator = anim;
        SubscribeCameraEvents();
    }


    public void SetPlayer(Player player)
    {
        this.player = player;
    }

    bool isLock = false;

    public void Lock()
    {
        isLock = true;
    }
    public void Unlock()
    {
        isLock = false;
    }
}

using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;



[RequireComponent(typeof(NavMeshAgent), typeof(Player))]
public class PlayerMovementResolver : MonoBehaviour
{
    Player player;
    PlayerInputReciever inputs;
    NavMeshAgent agent;
    Camera cam;

    MoveMode moveMode = MoveMode.Standing_Idle;
    const MoveMode CrouchBit = (MoveMode)0b00000001;

    Vector2 moveInput;
    Vector3 moveDirection;
    float fullInputTimer;
    float targetAnimVelocity;
    float currentVelocity;
    bool canMove = true;


    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float inputDeadzone = 0.1f;
    [SerializeField] float runRampTime = 0.5f;
    [SerializeField] float fullInputThreshold = 0.99f;
    [SerializeField] float walkAnimSpeed = 0.5f;
    [SerializeField] float runAnimSpeed = 1.0f;
    [SerializeField] float sprintAnimSpeed = 1.5f;
    [SerializeField] float holdTimeToRun = 1.0f;
    [SerializeField, Range(0.1f, 0.5f)] float crouchTimeout = 0.3f;
    public float CurrentVelocity => currentVelocity;
    public Vector2 CurrentHV { get; private set; }

    [SerializeField] UIDocument doc;


    void Awake()
    {
        player = GetComponent<Player>();
        inputs = GetComponent<PlayerInputReciever>();
        agent = GetComponent<NavMeshAgent>();
        cam = Camera.main;

        agent.updatePosition = false;
        agent.updateRotation = false;
    }

    bool isCrouching = false;
    bool canToggleCrouchMode = true;

    void ResetCanToggleCrouchMode()
    {
        canToggleCrouchMode = true;
    }

    void Update()
    {
        if (!canMove) return;

        moveInput = inputs.MoveInput;

        // Apply deadzone to raw input
        float deadzone = 0.05f;
        if (Mathf.Abs(moveInput.x) < deadzone) moveInput.x = 0f;
        if (Mathf.Abs(moveInput.y) < deadzone) moveInput.y = 0f;

        bool hasInput = HasMovementInput();

        CalculateMoveDirection();
        UpdateAimTarget();
        HandleRotation();

        if (hasInput)
        {
            UpdateMoveModeAndVelocity();
        }
        else
        {
            // Explicit idle handling
            SetIdleState();
        }

        // Crouch toggle logic still runs regardless of movement
        if (inputs.Crouch && canToggleCrouchMode)
        {
            canToggleCrouchMode = false;
            isCrouching = !isCrouching;
            Invoke(nameof(ResetCanToggleCrouchMode), crouchTimeout);
        }

        if (isCrouching)
            moveMode |= CrouchBit;
        else
            moveMode &= ~CrouchBit;

        player.SetCrouch(isCrouching);

        // Calculate local-space HV even when idle
        Vector3 localDir = transform.InverseTransformDirection(moveDirection);
        Vector2 hv = hasInput ? new Vector2(localDir.x, localDir.z) * currentVelocity : Vector2.zero;

        // Sprint scaling
        if ((moveMode & ~CrouchBit) == MoveMode.Sprint && hv.y >= 0.5f)
            hv.y = sprintAnimSpeed;

        // Apply animation deadzone to HV
        float animDeadzone = 0.01f;
        if (hv.magnitude < animDeadzone) hv = Vector2.zero;

        CurrentHV = hv;

        // Sprint event derived from final hv
        bool isSprintingAnim = hv.y >= sprintAnimSpeed - 0.01f;
        player.SetSprint(isSprintingAnim);

        bool isFocus = inputs.Focus;
        player.SetFocus(isFocus);
    }


    bool HasMovementInput() => moveInput.sqrMagnitude >= inputDeadzone * inputDeadzone;

    void SetIdleState()
    {
        moveMode = (moveMode & CrouchBit) == CrouchBit ? MoveMode.Crouching_Idle : MoveMode.Standing_Idle;
        currentVelocity = 0f;
    }

    void CalculateMoveDirection()
    {
        moveDirection = (cam.transform.forward * moveInput.y + cam.transform.right * moveInput.x);
        moveDirection.y = 0f;
        moveDirection.Normalize();
    }

    [SerializeField] Transform aimTarget;


    void HandleRotation()
    {
        Vector3 targetDir = Vector3.zero;

        if (inputs.Focus && inputs.IsMouse) 
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

            // Assume ground plane at player height
            Plane groundPlane = new Plane(Vector3.up, transform.position);
            if (groundPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                targetDir = hitPoint - transform.position;

                // Keep aimTarget synced to mouse hit
                aimTarget.position = hitPoint;
            }
        }
        // Priority 1: right stick / mouse aim
        else if (inputs.Focus && inputs.LookInput.sqrMagnitude > 0.01f)
        {
            targetDir = (aimTarget.position - transform.position);
        }
        // Priority 2: movement direction
        else if (HasMovementInput())
        {
            targetDir = moveDirection;
        }

        targetDir.y = 0;
        if (targetDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(targetDir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }
    }
    [SerializeField] float aimDistance = 20f; // how far in front of player the aim target sits

    void UpdateAimTarget()
    {
        if (!inputs.Focus)
        {
            aimTarget.position = transform.position;
            return;
        }
        Vector2 aimInput = inputs.LookInput;

        if (aimInput.sqrMagnitude < 0.01f)
        {
            aimTarget.position = Vector3.Lerp(
                aimTarget.position,
                transform.position,
                Time.deltaTime * 10f // tweak speed
            );
            return;
        }

        // --- 1. Convert stick input into screen-space direction ---
        Vector3 screenDir = new Vector3(aimInput.x, aimInput.y, 0f);

        // --- 2. Map screen-space to world-space ---
        Vector3 camForward = cam.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cam.transform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 worldDir = (camRight * screenDir.x + camForward * screenDir.y).normalized;

        // --- 3. Position aim target in world ---
        aimTarget.position = transform.position + worldDir * aimDistance;
    }


    void UpdateMoveModeAndVelocity()
    {
        bool sprinting = inputs.Sprint;
        bool atFullInput = moveInput.magnitude >= fullInputThreshold;

        if (sprinting)
        {
            targetAnimVelocity = sprintAnimSpeed;
            moveMode = (moveMode & CrouchBit) == CrouchBit ? MoveMode.Crouching_Sprint : MoveMode.Sprint;
        }
        else
        {
            // If we were sprinting last frame, drop back to run/walk immediately
            if ((moveMode & ~CrouchBit) == MoveMode.Sprint || (moveMode & ~CrouchBit) == MoveMode.Crouching_Sprint)
            {
                // Force downshift
                targetAnimVelocity = runAnimSpeed;
                moveMode = (moveMode & CrouchBit) == CrouchBit ? MoveMode.Crouching_Run : MoveMode.Run;
            }

            if (!atFullInput || currentVelocity < walkAnimSpeed)
            {
                fullInputTimer = 0f;
                targetAnimVelocity = walkAnimSpeed;
                moveMode = (moveMode & CrouchBit) == CrouchBit ? MoveMode.Crouching_Walk : MoveMode.Walk;
            }

            if (atFullInput)
            {
                fullInputTimer += Time.deltaTime;
                if (fullInputTimer >= holdTimeToRun)
                {
                    targetAnimVelocity = runAnimSpeed;
                    moveMode = (moveMode & CrouchBit) == CrouchBit ? MoveMode.Crouching_Run : MoveMode.Run;
                }
            }
        }

        currentVelocity = Mathf.MoveTowards(currentVelocity, targetAnimVelocity, Time.deltaTime / runRampTime);
    }

    internal void Lock()
    {
        canMove = false;
    }
}

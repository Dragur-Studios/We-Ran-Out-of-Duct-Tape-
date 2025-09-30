using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationResolver : MonoBehaviour
{
    Player player;
    PlayerMovementResolver movement;
    Animator animator;

    int CROUCH_HASH;
    int VELOCITY_HASH;
    int HORIZONTAL_HASH;
    int VERTICAL_HASH;
    int INPUT_DETECT_HASH;

    [SerializeField] float animSmoothTime = 0.1f;

    void Awake()
    {
        player = GetComponentInParent<Player>();
        movement = player.GetComponent<PlayerMovementResolver>();
        animator = GetComponent<Animator>();

        CROUCH_HASH = Animator.StringToHash("isCrouching");
        VELOCITY_HASH = Animator.StringToHash("Velocity");
        HORIZONTAL_HASH = Animator.StringToHash("Horizontal");
        VERTICAL_HASH = Animator.StringToHash("Vertical");
        INPUT_DETECT_HASH = Animator.StringToHash("isInput");
    }

    void OnEnable()
    {
        player.OnCrouch += val => animator.SetBool(CROUCH_HASH, val);
    }

    void OnDisable()
    {
        player.OnCrouch -= val => animator.SetBool(CROUCH_HASH, val);
    }

    void Update()
    {
        float velocity = movement.CurrentVelocity;
        Vector2 hv = movement.CurrentHV;

        animator.SetFloat(VELOCITY_HASH, velocity);
        animator.SetFloat(HORIZONTAL_HASH, hv.x, animSmoothTime, Time.deltaTime);
        animator.SetFloat(VERTICAL_HASH, hv.y, animSmoothTime, Time.deltaTime);
        animator.SetBool(INPUT_DETECT_HASH, hv.sqrMagnitude > 0.01f);
    }

    public void OnFootstep(float baseIntensity)
    {
        // Clamp to a very subtle range
        float intensity = Mathf.Clamp(baseIntensity, 0.001f, 0.005f);

        var pkt = new FeedbackPacket();
        pkt.IsGamepad = Gamepad.current != null;

        // Only low motor for a "thud"
        pkt.GamepadMotorLowFrequency = intensity;
        pkt.GamepadMotorHighFrequency = 0f;

        // Very short pulse
        pkt.GamepadMotorFeedbackDuration = Random.Range(0.05f, 0.08f);

        Haptics.ApplyFeedback(pkt);
    }


    private void OnAnimatorMove()
    {
        
    }
}

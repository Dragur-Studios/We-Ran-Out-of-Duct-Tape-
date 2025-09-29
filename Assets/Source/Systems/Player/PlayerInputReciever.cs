using UnityEngine;

public class PlayerInputReciever : MonoBehaviour
{
    PlayerControls controls;

    Vector2 _moveInput;
    Vector2 _lookInput;

    bool _crouch;
    bool _focus;
    bool _sprint;
    bool _interact;
    bool _fire;

    public Vector2 MoveInput { get => _moveInput; }
    public Vector2 LookInput { get => _lookInput; }

    public bool Crouch { get => _crouch; }
    public bool Focus { get => _focus; }
    public bool Sprint { get => _sprint; }
    public bool Fire { get => _fire; }

    public bool Interact { get => _interact; }
    private void OnEnable()
    {
        if(controls == null)
        {
            controls = new PlayerControls();

            controls.Enable();
        }
    }

    private void Update()
    {
        _moveInput = controls.Player.Move.ReadValue<Vector2>();
        _lookInput = controls.Player.Look.ReadValue<Vector2>();
        _crouch = controls.Player.Crouch.ReadValue<float>() > 0.5f;
        _focus = controls.Player.Focus.ReadValue<float>() > 0.5f;
        _sprint = controls.Player.Sprint.ReadValue<float>() > 0.5f;
        _interact = controls.Player.Interact.ReadValue<float>() > 0.5f;
        _fire = controls.Player.Fire.ReadValue<float>() > 0.5f;

    }


}

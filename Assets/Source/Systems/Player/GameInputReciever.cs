using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputReciever : MonoBehaviour
{
    public static GameInputReciever Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    PlayerControls controls;

    #region ON FOOT CONTROLS

    Vector2 _moveInput;
    Vector2 _lookInput;

    bool _crouch;
    bool _focus;
    bool _sprint;
    bool _interact; // aka _enter.
    bool _fire;
    bool _itemSlot1;
    bool _itemSlot2;
    bool _itemSlot3;
    bool _itemSlot4;

    bool _pauseFlag;

    float _rotateCamera;

    public Vector2 MoveInput { get => _moveInput; }
    public Vector2 LookInput { get => _lookInput; }
    public float CameraRotation { get => _rotateCamera; }

    public bool Crouch { get => _crouch; }
    public bool Focus { get => _focus; }
    public bool Sprint { get => _sprint; }
    public bool Fire { get => _fire; }

    public bool Interact { get => _interact; }
    public bool ItemSlot01 { get => _itemSlot1; }
    public bool ItemSlot02 { get => _itemSlot2; }
    public bool ItemSlot03 { get => _itemSlot3; }
    public bool ItemSlot04 { get => _itemSlot4; }

    public bool PauseFlag { get => _pauseFlag; }

    public bool IsMouse { get; private set; }

    #endregion

    #region VEHICLE CONTROLS
    float _gas;
    float _brake;
    bool _lights;
    float _steer;
    bool _exit;

    public float Gas { get => _gas; }
    public float Brake { get => _brake; }
    public bool Lights { get => _lights; }
    public float Steering { get => _steer; }    
    public bool Exit { get => _exit; }

    #endregion
    private void OnEnable()
    {
        if(controls == null)
        {
            controls = new PlayerControls();
            controls.Player.Look.performed += ctx =>
            {
                var device = ctx.control.device;
                if (device is Mouse)
                    IsMouse = true;
                else if (device is Gamepad)
                    IsMouse = false;
            };


            controls.Player.TryPause.performed += ctx =>
            {
                if(ctx.ReadValue<float>()> 0.5f)
                {
                    GameManager.TryPause();
                }
            };


            controls.Enable();
        }
    }
    public enum ControlSet
    {
        OnFoot,
        Vehicle
    }
    public void EnableControls(ControlSet controlSet)
    {
        //switch (controlSet)
        //{
        //    case ControlSet.OnFoot:
        //        controls.Vehicle.Disable();
        //        controls.Player.Enable();

        //        break;
        //    case ControlSet.Vehicle:
        //        controls.Vehicle.Enable();
        //        controls.Player.Disable();
        //        break;
        //}
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
        _rotateCamera = controls.Player.RotateCamera.ReadValue<float>();

        _itemSlot1 = controls.Player.ItemSlot1.ReadValue<float>() > 0.5f;
        _itemSlot2 = controls.Player.ItemSlot2.ReadValue<float>() > 0.5f;
        _itemSlot3 = controls.Player.ItemSlot3.ReadValue<float>() > 0.5f;
        _itemSlot4 = controls.Player.ItemSlot4.ReadValue<float>() > 0.5f;


        _brake = controls.Vehicle.Brake_Reverse.ReadValue<float>();
        _gas = controls.Vehicle.Gas.ReadValue<float>();
        _exit = controls.Vehicle.Exit.ReadValue<float>() != 0;
        _steer = controls.Vehicle.Steer.ReadValue<float>();

        _lights = controls.Vehicle.Lights.ReadValue<float>() != 0;


    }


}

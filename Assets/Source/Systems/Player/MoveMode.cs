
//using UnityEngine.InputSystem;

public enum MoveMode
{
    None = -1,
    Standing_Idle = 0b00000000, Crouching_Idle = 0b00000001,
    Walk = 0b00000010, Crouching_Walk = 0b00000011,
    Run = 0b00000100, Crouching_Run = 0b00000101,
    Sprint = 0b00001000, Crouching_Sprint = 0b00001001,
}

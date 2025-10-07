using UnityEngine;

public class VehicleCameraHandler : MonoBehaviour
{
    [SerializeField] private Transform cinemachineAnchor; // Pivot for the camera
    [SerializeField] private float sensitivityX = 150f;
    [SerializeField] private float sensitivityY = 100f;
    [SerializeField] private float transitionSpeed = 5.0f;

    private float yaw;   // Horizontal rotation

    GameInputReciever inputs;


    private void Start()
    {
        inputs = GameInputReciever.Instance;
    }

    float sk_posT = 0;

    void LateUpdate()
    {
        if (isLock) return;

        // --- 1. Read input ---
        var rotate = inputs.CameraRotation;

        yaw += rotate * sensitivityX * Time.deltaTime;


        // --- 4. Apply rotation to anchor ---
        cinemachineAnchor.rotation = Quaternion.Euler(0, yaw, 0f);



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

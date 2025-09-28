using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera mainCam;

    void Start()
    {
        // Cache the main camera reference
        mainCam = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCam == null) return;

        // Make the object's BACK (Z+) point toward the camera
        Vector3 directionToCamera = mainCam.transform.position - transform.position;

        // Invert the direction so the FRONT faces the camera
        directionToCamera = -directionToCamera;

        // Apply rotation
        transform.rotation = Quaternion.LookRotation(directionToCamera, Vector3.up);
    }
}

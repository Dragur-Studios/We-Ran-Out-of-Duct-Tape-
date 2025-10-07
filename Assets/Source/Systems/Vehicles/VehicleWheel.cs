using UnityEngine;

[System.Serializable]
public class VehicleWheel
{
    public GameObject GameObject;
    public WheelCollider Collider;

    public void Sync()
    {
        if (Collider == null || GameObject == null) return;

        Collider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        GameObject.transform.position = pos;
        GameObject.transform.rotation = rot;
    }

    public void ApplyTraction(WheelTractionSettings traction)
    {
        if (Collider == null) return;

        WheelFrictionCurve forward = Collider.forwardFriction;
        WheelFrictionCurve sideways = Collider.sidewaysFriction;

        forward.stiffness = traction.forwardStiffness;
        sideways.stiffness = traction.sidewaysStiffness;

        Collider.forwardFriction = forward;
        Collider.sidewaysFriction = sideways;
    }
}


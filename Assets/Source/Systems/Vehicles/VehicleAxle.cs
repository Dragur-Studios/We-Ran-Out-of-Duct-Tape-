public enum AxelLocation
{
    Front,
    Rear
}
[System.Serializable]
public class VehicleAxle
{
    public AxelLocation Location;
    public VehicleWheel[] Wheels;
    public bool steering;   // Can this axle steer?
    public bool motor;      // Can this axle receive torque?
}


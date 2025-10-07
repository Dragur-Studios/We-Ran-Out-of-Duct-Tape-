using UnityEngine;

public class GameVehicleManager : MonoBehaviour
{
    public static GameVehicleManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public static void EnterVehicle(VehicleController vehicle)
    {
        var player = GameManager.Instance.Player;
        player.OnEnterVehicle();
        
        player.transform.SetParent(vehicle.transform);
        player.transform.position = Vector3.zero;
        player.transform.rotation = Quaternion.identity;
        player.gameObject.SetActive(false);


        var input = GameInputReciever.Instance;
        input.EnableControls(GameInputReciever.ControlSet.Vehicle); // switch to vehicle controls.

        GameCamera.Track(vehicle.FollowTarget);

        vehicle.TurnOn();
        // finally turn the vehicle on. 
    }

    public static void ExitVehicle(VehicleController vehicle)
    {
        var player = GameManager.Instance.Player;
        player.OnExitVehicle();

        player.transform.SetParent(null);
        player.gameObject.SetActive(true);

        var input = GameInputReciever.Instance;
        input.EnableControls(GameInputReciever.ControlSet.OnFoot);

        GameManager.Instance.TrackPlayer();
    }


}

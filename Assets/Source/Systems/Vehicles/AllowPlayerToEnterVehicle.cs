using UnityEngine;

public class AllowPlayerToEnterVehicle : MonoBehaviour
{
    VehicleController controller;

    private void Start()
    {
        controller = GetComponentInParent<VehicleController>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Debug.Log("Interact");
            var interactHandler = other.GetComponent<PlayerInteractionHandler>();
            interactHandler.QueueVehicleEnter(controller);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            Debug.Log("ooo.. just missed it..");
            var interactHandler = other.GetComponent<PlayerInteractionHandler>();
            interactHandler.CancelVehicleEnter();
        }
    }
}

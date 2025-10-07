using System;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerInteractionHandler : MonoBehaviour
{

    [SerializeField] UIDocument playerHUD;
    VisualElement root;
    VisualElement interactPopup;

    private GameInputReciever inputs;

    VehicleController enterVehicle = null;

    bool canInteract = true;
    Player player;

    private void Start()
    {
        player= GetComponent<Player>();

        root = playerHUD.rootVisualElement;

        VisualTreeAsset popup = Resources.Load<VisualTreeAsset>("User Interface/Components/Interaction_Popup");
        interactPopup = popup.Instantiate();
        interactPopup.style.position = Position.Absolute;
        interactPopup.style.display = DisplayStyle.None;

        root.Add(interactPopup);

        inputs = player.Input;
    }

    private void Update()
    {
        HandleInteraction();
    }

    void HidePopup()
    {
        interactPopup.style.display = DisplayStyle.None;
    }
    void ShowPopup()
    {
        interactPopup.style.display = DisplayStyle.Flex;
    }
    private void HandleInteraction()
    {
        // Poll input from your PlayerInputReciever
        if (inputs.Interact && canInteract) 
        {
            canInteract = false;

            if(enterVehicle != null) 
            {
                GameVehicleManager.EnterVehicle(enterVehicle);
            }

            HidePopup();
            
            Invoke(nameof(ResetCanInteract), 1.0f);
        }
    }
     
    void ResetCanInteract()
    {
        canInteract = true;
    }

  

    public void QueueVehicleEnter(VehicleController vehicle)
    {
        enterVehicle = vehicle;

        var wp = vehicle.transform.position;
        var posScreen = GameCamera.GetCamera().WorldToScreenPoint(wp);

        // Flip Y because UI Toolkit's origin is top-left
        float uiX = posScreen.x;
        float uiY = posScreen.y;

        interactPopup.style.left = uiX;
        interactPopup.style.top = uiY;

        ShowPopup();
    }

    public void CancelVehicleEnter()
    {
        HidePopup();
        enterVehicle = null;
    }
}

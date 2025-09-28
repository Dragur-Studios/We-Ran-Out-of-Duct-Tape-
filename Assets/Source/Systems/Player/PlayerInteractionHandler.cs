using UnityEngine;
using UnityEngine.UIElements;

public class PlayerInteractionHandler : MonoBehaviour
{
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private float interactThreshold = 0.2f;
    [SerializeField] private LayerMask interactableMask;

    [SerializeField] UIDocument playerHUD;
    VisualElement root;
    VisualElement interactPopup;

    private Camera mainCam;
    private PlayerInputReciever inputs;
    private IInteractable curInteract;
    bool canInteract = true;

    private void Start()
    {
        root = playerHUD.rootVisualElement;

        VisualTreeAsset popup = Resources.Load<VisualTreeAsset>("Interaction_Popup");
        interactPopup = popup.Instantiate();
        interactPopup.style.position = Position.Absolute;
        interactPopup.style.display = DisplayStyle.None;

        root.Add(interactPopup);

        mainCam = Camera.main;
        inputs = GetComponent<PlayerInputReciever>();
    }

    private void Update()
    {
        HandleDetection();
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

    private void HandleDetection()
    {
        // Raycast from camera center forward
        Ray ray = new Ray(mainCam.transform.position, mainCam.transform.forward);

        if (Physics.SphereCast(ray, interactThreshold, out var hit,  interactRange, interactableMask))
        {
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                if (curInteract != interactable)
                {
                    ClearCurrentInteract();
                    curInteract = interactable;
                }

                curInteract.isInteractAvailable = true;

                var wp = curInteract.transform.position;
                var posScreen = mainCam.WorldToScreenPoint(hit.point);

                // Flip Y because UI Toolkit's origin is top-left
                float uiX = posScreen.x;
                float uiY = posScreen.y;

                interactPopup.style.left = uiX;
                interactPopup.style.top = uiY;

                ShowPopup();


                ShowPopup();
                return;
            }
        }

        // If we didn't hit a door, clear the old one
        ClearCurrentInteract();
    }
    private void HandleInteraction()
    {
        // Poll input from your PlayerInputReciever
        if (inputs.Interact && canInteract) 
        {
            canInteract = false;

            if (curInteract != null)
            {
                curInteract.TryInteract();
            }

            HidePopup();
            
            Invoke(nameof(ResetCanInteract), 1.0f);
        }
    }
     
    void ResetCanInteract()
    {
        canInteract = true;
    }

    private void ClearCurrentInteract()
    {
        if (curInteract != null)
        {
            HidePopup();
            curInteract.isInteractAvailable = false;
            curInteract = null;
        }
    }
}

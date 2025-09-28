using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum DoorLockType  
{
    None, // no lock on this door, can just be opened or closed.
    
    Key_Lock,

}

public class DoorHandle : IInteractable
{
    [SerializeField] Animator doorAnim;

    [SerializeField] DoorLockType lockType;

    public float animationTime = 1.0f;


    private void Start()
    {
        OnInteract += () =>
        {
            bool isValidInteraction = true;

            switch (lockType)
            {
                case DoorLockType.Key_Lock:
                    {
                        if (!PlayerHasKey())
                        {
                            isValidInteraction = false;                
                        }
                    }
                    break;

            }

            if (!isValidInteraction)
                return;

            doorAnim.SetTrigger("Interact");
        };
    }

    
    bool PlayerHasKey()
    {
        //var player = GameManager.Instance.Player;
        return true;
    }
   
}

using System;
using UnityEngine;

public class IInteractable : MonoBehaviour {

    //[SerializeField] GameObject Icon;

    public Action OnInteract;


    public bool isInteractAvailable = false;

    public bool TryInteract()
    {
        if (!isInteractAvailable)
            return false;

        HandleInteract();

        return true;
    }

    protected void HandleInteract()
    {
        OnInteract?.Invoke();
    }
    private void Update()
    {
        //Icon?.SetActive(isInteractAvailable);
    }

}

using System;
using UnityEngine;

public class Key : IItem
{
    private void Start()
    {
        guid = $"key_{simple_guid()}";

        OnInteract += () =>
        {
            GameManager.Instance.Player.PickUp(this);
        };
    }

    string simple_guid()
    {
        return Guid.NewGuid().ToString();
    }
}

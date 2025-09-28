using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    List<PlayerInventoryItem> items = new List<PlayerInventoryItem>();

    public void Insert(IItem item, int quantity=1)
    {
        var found = items.Where(i => i.Item.guid == item.guid).FirstOrDefault();

        if(found != null)
        {
            found.Quantity += quantity;
            return;
        }

        items.Add(new PlayerInventoryItem { Item = item, Quantity = quantity });
    }

}

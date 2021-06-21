using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : AbstractInventory, ICollectible
{
    public void Collect(IInventoryStorage otherStorage)
    {
        foreach (var item in Storage.Items)
            otherStorage.Add(item.item, item.count);

        Storage.Clear();

        Destroy(gameObject);
    }
}

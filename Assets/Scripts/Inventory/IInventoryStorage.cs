using System;
using System.Collections.Generic;

public struct InventoryStorageItem
{
    public InventoryItem item;
    public int count;

    public InventoryStorageItem(InventoryItem item, int count)
    {
        this.item = item;
        this.count = count;
    }
}

public interface IInventoryStorage
{
    event Action OnChanged;

    IEnumerable<InventoryStorageItem> Items { get; }

    int CountOf(InventoryItem item);

    void Add(InventoryItem item, int amount = 1);
    bool Remove(InventoryItem item, int amount = 1);

    void Clear();
}

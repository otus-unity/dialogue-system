using System;
using System.Collections.Generic;

public interface IInventoryStorage
{
    event Action OnChanged;

    IEnumerable<(InventoryItem item, int count)> Items { get; }

    int CountOf(InventoryItem item);

    void Add(InventoryItem item, int amount = 1);
    bool Remove(InventoryItem item, int amount = 1);

    void Clear();
}

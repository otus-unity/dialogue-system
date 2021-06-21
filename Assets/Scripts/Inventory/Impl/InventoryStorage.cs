using UnityEngine;
using System;
using System.Collections.Generic;

public class InventoryStorage : IInventoryStorage
{
    sealed class Comparer : IComparer<InventoryItem>
    {
        public static readonly Comparer Instance = new Comparer();

        public int Compare(InventoryItem a, InventoryItem b)
        {
            if (a.LessThan(b))
                return -1;
            if (b.LessThan(a))
                return 1;

            int iidA = a.GetInstanceID();
            int iidB = b.GetInstanceID();
            if (iidA < iidB)
                return -1;
            if (iidA > iidB)
                return 1;

            return 0;
        }
    }

    readonly SortedDictionary<InventoryItem, int> m_items = new SortedDictionary<InventoryItem, int>(Comparer.Instance);

    public event Action OnChanged;
    public int Count => m_items.Count;

    public IEnumerable<InventoryStorageItem> Items { get {
            foreach (var it in m_items)
                yield return new InventoryStorageItem(it.Key, it.Value);
        } }

    public int CountOf(InventoryItem item)
    {
        m_items.TryGetValue(item, out int count);
        return count;
    }

    public void Add(InventoryItem item, int amount)
    {
        if (amount <= 0) {
            Debug.LogError($"Attempted to add {amount} of '{item.Title}' into the inventory.");
            return;
        }

        m_items.TryGetValue(item, out int count);
        m_items[item] = count + amount;

        OnChanged?.Invoke();
    }

    public bool Remove(InventoryItem item, int amount)
    {
        if (amount <= 0) {
            Debug.LogError($"Attempted to remove {amount} of '{item.Title}' from the inventory.");
            return false;
        }

        if (!m_items.TryGetValue(item, out int count) || count < amount)
            return false;

        count -= amount;
        if (count > 0)
            m_items[item] = count;
        else {
            if (!m_items.Remove(item))
                return false;
        }

        OnChanged?.Invoke();
        return true;
    }

    public void Clear()
    {
        m_items.Clear();
        OnChanged?.Invoke();
    }
}

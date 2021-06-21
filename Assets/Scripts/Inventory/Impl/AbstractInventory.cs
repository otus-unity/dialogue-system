using System;
using UnityEngine;

public abstract class AbstractInventory : MonoBehaviour, IInventory
{
    [Serializable]
    public struct InitialItem
    {
        public InventoryItem Item;
        public int Count;
    }

    public IInventoryStorage Storage { get; private set; } = new InventoryStorage();

    [SerializeField] InitialItem[] m_initialInventory;

    void Awake()
    {
        if (m_initialInventory == null)
            return;

        foreach (var it in m_initialInventory) {
            if (it.Count <= 0)
                Debug.LogError($"Invalid count {it.Count} in inventory of '{gameObject.name}'.");
            else
                Storage.Add(it.Item, it.Count);
        }
    }
}

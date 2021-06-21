using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class InventoryRow : MonoBehaviour, IPoolable<IMemoryPool>, IDisposable
{
    public class Factory : PlaceholderFactory<InventoryRow>
    {
    }

    [Inject] InventoryIcon.Factory m_iconFactory = default;

    IMemoryPool m_pool;
    readonly List<InventoryIcon> m_icons = new List<InventoryIcon>();

    public int Count => m_icons.Count;

    void Awake()
    {
    }

    public void Dispose()
    {
        if (m_pool != null) {
            m_pool.Despawn(this);
            m_pool = null;
        }
    }

    public void OnSpawned(IMemoryPool pool)
    {
        m_pool = pool;
    }

    public void OnDespawned()
    {
        Clear();
    }

    public void AddItem(InventoryItem item, int count)
    {
        var icon = m_iconFactory.Create(item, count);
        icon.transform.SetParent(transform, false);
        m_icons.Add(icon);
    }

    public void Clear()
    {
        foreach (var icon in m_icons)
            icon.Dispose();
        m_icons.Clear();
    }
}

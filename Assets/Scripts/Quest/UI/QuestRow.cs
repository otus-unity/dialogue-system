using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class QuestRow : MonoBehaviour, IPoolable<Quest, IMemoryPool>, IDisposable
{
    public class Factory : PlaceholderFactory<Quest, QuestRow>
    {
    }

    [SerializeField] TextMeshProUGUI m_text;
    IMemoryPool m_pool;

    public void Dispose()
    {
        if (m_pool != null) {
            m_pool.Despawn(this);
            m_pool = null;
        }
    }

    public void OnSpawned(Quest quest, IMemoryPool pool)
    {
        m_text.text = quest.Name;
        m_pool = pool;
    }

    public void OnDespawned()
    {
    }
}

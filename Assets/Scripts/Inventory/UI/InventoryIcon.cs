using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Image))]
public class InventoryIcon : MonoBehaviour, IPoolable<InventoryItem, int, IMemoryPool>, IDisposable
{
    public class Factory : PlaceholderFactory<InventoryItem, int, InventoryIcon>
    {
    }

    Image m_image;
    IMemoryPool m_pool;

    [SerializeField] GameObject m_overlay;
    [SerializeField] TextMeshProUGUI m_text;

    void Awake()
    {
        m_image = GetComponent<Image>();
    }

    public void Dispose()
    {
        if (m_pool != null) {
            m_pool.Despawn(this);
            m_pool = null;
        }
    }

    public void OnSpawned(InventoryItem item, int count, IMemoryPool pool)
    {
        m_pool = pool;
        m_image.sprite = item.Icon;

        if (count == 1)
            m_overlay.SetActive(false);
        else {
            m_text.text = count.ToString();
            m_overlay.SetActive(true);
        }
    }

    public void OnDespawned()
    {
    }
}

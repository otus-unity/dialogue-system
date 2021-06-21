using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Button))]
public class InventoryCategoryButton : MonoBehaviour,
    IPoolable<InventoryCategory, IMemoryPool>, IDisposable
{
    public class Factory : PlaceholderFactory<InventoryCategory, InventoryCategoryButton>
    {
    }

    Button m_button;
    IMemoryPool m_pool;

    [SerializeField] TextMeshProUGUI m_text;

    public InventoryCategory Category { get; private set; }
    public event Action OnClicked;

    void Awake()
    {
        m_button = GetComponent<Button>();
        m_button.onClick.AddListener(() => OnClicked?.Invoke());
    }

    public void Dispose()
    {
        if (m_pool != null) {
            m_pool.Despawn(this);
            m_pool = null;
        }

        OnClicked = null;
    }

    public void OnSpawned(InventoryCategory category, IMemoryPool pool)
    {
        m_pool = pool;
        m_text.text = category.Name;
    }

    public void OnDespawned()
    {
    }
}

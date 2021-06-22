using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

[RequireComponent(typeof(Button))]
public class DialogButton : MonoBehaviour, IPoolable<string, IMemoryPool>, IDisposable
{
    public sealed class Factory : PlaceholderFactory<string, DialogButton>
    {
    }

    public bool Selected { get; private set; }

    IMemoryPool m_pool;
    Button m_button;
    [SerializeField] TextMeshProUGUI m_text;

    void Awake()
    {
        m_button = GetComponent<Button>();
    }

    public void Dispose()
    {
        if (m_pool != null) {
            m_pool.Despawn(this);
            m_pool = null;
        }
    }

    public void OnSpawned(string text, IMemoryPool pool)
    {
        m_pool = pool;
        m_text.text = text;
        Selected = false;
    }

    public void OnDespawned()
    {
    }

    public void Select()
    {
        Selected = true;
    }
}

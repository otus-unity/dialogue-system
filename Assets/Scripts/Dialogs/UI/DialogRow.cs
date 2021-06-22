using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Zenject;

public sealed class DialogRow : MonoBehaviour, IPoolable<IMemoryPool>, IDisposable
{
    public sealed class Factory : PlaceholderFactory<DialogRow>
    {
    }

    struct ButtonInfo
    {
        public readonly DialogButton Button;
        public readonly DialogNode Node;

        public ButtonInfo(DialogButton button, DialogNode node)
        {
            Button = button;
            Node = node;
        }
    }

    public TextMeshProUGUI Text;
    [SerializeField] Transform m_contents;
    public Transform BalloonImage;

    IMemoryPool m_pool;
    readonly List<ButtonInfo> m_buttons = new List<ButtonInfo>();

    [Inject] DialogButton.Factory m_buttonFactory = default;

    public void OnSpawned(IMemoryPool pool)
    {
        m_pool = pool;
    }

    public void OnDespawned()
    {
        DestroyButtons();
    }

    public void Dispose()
    {
        if (m_pool != null) {
            m_pool.Despawn(this);
            m_pool = null;
        }
    }

    public void AddButton(string id, string text, DialogNode node)
    {
        var button = m_buttonFactory.Create(text);
        button.transform.SetParent(m_contents.transform);
        m_buttons.Add(new ButtonInfo(button, node));
    }

    public void DestroyButtons()
    {
        foreach (var it in m_buttons)
            it.Button.Dispose();
        m_buttons.Clear();
    }

    public bool TryGetSelected(out DialogNode node)
    {
        foreach (var it in m_buttons) {
            if (it.Button.Selected) {
                node = it.Node;
                return true;
            }
        }

        node = null;
        return false;
    }
}

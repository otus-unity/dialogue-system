using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] AbstractInventory m_inventory;
    [SerializeField] Transform m_buttonArea;
    [SerializeField] Transform m_content;
    [SerializeField] int m_itemsPerRow = 8;

    readonly List<InventoryCategoryButton> m_buttons = new List<InventoryCategoryButton>();
    [Inject] InventoryCategoryButton.Factory m_buttonFactory = default;

    readonly List<InventoryRow> m_rows = new List<InventoryRow>();
    [Inject] InventoryRow.Factory m_rowFactory = default;

    void OnEnable()
    {
        m_inventory.Storage.OnChanged += Refresh;
        Refresh();
    }

    void OnDisable()
    {
        m_inventory.Storage.OnChanged -= Refresh;
        DestroyRows();
        DestroyButtons();
    }

    void Refresh()
    {
        DestroyRows();
        FillCategories();
    }

    void FillCategories()
    {
        DestroyButtons();

        HashSet<InventoryCategory> categories = new HashSet<InventoryCategory>();
        foreach (var it in m_inventory.Storage.Items)
            categories.Add(it.item.Category);

        foreach (var cat in categories) {
            var button = m_buttonFactory.Create(cat);
            button.transform.SetParent(m_buttonArea, false);
            button.OnClicked += () => FillItems(cat);
            m_buttons.Add(button);
        }
    }

    void FillItems(InventoryCategory category)
    {
        DestroyRows();

        InventoryRow row = null;
        foreach (var it in m_inventory.Storage.Items) {
            if (it.item.Category != category)
                continue;

            if (row == null || row.Count >= m_itemsPerRow) {
                row = m_rowFactory.Create();
                row.transform.SetParent(m_content, false);
                m_rows.Add(row);
            }

            row.AddItem(it.item, it.count);
        }
    }

    void DestroyButtons()
    {
        foreach (var button in m_buttons)
            button.Dispose();
        m_buttons.Clear();
    }

    void DestroyRows()
    {
        foreach (var row in m_rows)
            row.Dispose();
        m_rows.Clear();
    }
}

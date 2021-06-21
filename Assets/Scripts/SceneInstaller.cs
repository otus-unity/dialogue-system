using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField] int m_inventoryRowPoolSize = 8;
    [SerializeField] InventoryRow m_inventoryRowPrefab;

    [SerializeField] int m_inventoryCategoryButtonPoolSize = 8;
    [SerializeField] InventoryCategoryButton m_inventoryCategoryButtonPrefab;

    [SerializeField] int m_inventoryIconPoolSize = 64;
    [SerializeField] InventoryIcon m_inventoryIconPrefab;

    public override void InstallBindings()
    {
        Container.BindFactory<InventoryRow, InventoryRow.Factory>()
            .FromMonoPoolableMemoryPool<InventoryRow>(opts => opts
                .WithInitialSize(m_inventoryRowPoolSize)
                .FromComponentInNewPrefab(m_inventoryRowPrefab)
                .UnderTransform(transform));

        Container.BindFactory<InventoryItem, int, InventoryIcon, InventoryIcon.Factory>()
            .FromMonoPoolableMemoryPool<InventoryItem, int, InventoryIcon>(opts => opts
                .WithInitialSize(m_inventoryIconPoolSize)
                .FromComponentInNewPrefab(m_inventoryIconPrefab)
                .UnderTransform(transform));

        Container.BindFactory<InventoryCategory, InventoryCategoryButton, InventoryCategoryButton.Factory>()
            .FromMonoPoolableMemoryPool<InventoryCategory, InventoryCategoryButton>(opts => opts
                .WithInitialSize(m_inventoryCategoryButtonPoolSize)
                .FromComponentInNewPrefab(m_inventoryCategoryButtonPrefab)
                .UnderTransform(transform));
    }
}

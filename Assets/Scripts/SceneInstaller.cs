using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField] DialogUI m_dialogUI;

    [SerializeField] int m_inventoryRowPoolSize = 8;
    [SerializeField] InventoryRow m_inventoryRowPrefab;

    [SerializeField] int m_inventoryCategoryButtonPoolSize = 8;
    [SerializeField] InventoryCategoryButton m_inventoryCategoryButtonPrefab;

    [SerializeField] int m_inventoryIconPoolSize = 64;
    [SerializeField] InventoryIcon m_inventoryIconPrefab;

    [SerializeField] int m_questRowPoolSize = 32;
    [SerializeField] QuestRow m_questRowPrefab;

    [SerializeField] int m_dialogButtonPoolSize = 8;
    [SerializeField] DialogButton m_dialogButtonPrefab;

    [SerializeField] int m_dialogRowPoolSize = 8;
    [SerializeField] DialogRow m_dialogRowPrefab;

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

        Container.BindFactory<Quest, QuestRow, QuestRow.Factory>()
            .FromMonoPoolableMemoryPool<Quest, QuestRow>(opts => opts
                .WithInitialSize(m_questRowPoolSize)
                .FromComponentInNewPrefab(m_questRowPrefab)
                .UnderTransform(transform));

        Container.BindFactory<string, DialogButton, DialogButton.Factory>()
            .FromMonoPoolableMemoryPool<string, DialogButton>(opts => opts
                .WithInitialSize(m_dialogButtonPoolSize)
                .FromComponentInNewPrefab(m_dialogButtonPrefab)
                .UnderTransform(transform));

        Container.BindFactory<DialogRow, DialogRow.Factory>()
            .FromMonoPoolableMemoryPool<DialogRow>(opts => opts
                .WithInitialSize(m_dialogRowPoolSize)
                .FromComponentInNewPrefab(m_dialogRowPrefab)
                .UnderTransform(transform));
    }
}

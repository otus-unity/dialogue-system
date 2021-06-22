using UnityEngine;

[CreateAssetMenu(menuName = "OTUS/Quest Conditions/Collect Item")]
public class QuestCollectItemsCondition : QuestCondition
{
    //[Inject(Id=Ids.Player)] IInventory m_playerInventory = default;

    [SerializeField] InventoryItem m_item;
    [SerializeField] int m_count;

    public override bool IsTrue()
    {
        // FIXME: не делайте так!!!
        var m_playerInventory = FindObjectOfType<CharacterInventory>();

        return m_playerInventory.Storage.CountOf(m_item) >= m_count;
    }
}

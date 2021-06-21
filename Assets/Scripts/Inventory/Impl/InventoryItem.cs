using UnityEngine;

[CreateAssetMenu(menuName = "OTUS/Inventory Item")]
public class InventoryItem : ScriptableObject
{
    public InventoryCategory Category;
    public string Title;
    public string Description;
    public Sprite Icon;

    public bool LessThan(InventoryItem other)
    {
        return Title.CompareTo(other.Title) < 0;
    }
}

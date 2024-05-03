using UnityEngine;
using EnumType;

[CreateAssetMenu(menuName = "Item/Consumable", fileName = "Item_Consumable_")]
public class ConsumableItemData : CountableItemData
{
    [field: SerializeField]
    public int RequiredCount { get; private set; } = 1;

    public ConsumableItemData()
    {
        ItemType = ItemType.Consumable;
    }

    public override Item CreateItem()
    {
        return new ConsumableItem(this);
    }

    public override CountableItem CreateItem(int count)
    {
        return new ConsumableItem(this, count);
    }
}

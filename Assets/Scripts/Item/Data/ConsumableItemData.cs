using UnityEngine;
using EnumType;

public abstract class ConsumableItemData : CountableItemData
{
    [field: SerializeField]
    public int RequiredCount { get; private set; } = 1;

    public ConsumableItemData()
    {
        ItemType = ItemType.Consumable;
    }

    public abstract override Item CreateItem();
    public abstract override CountableItem CreateItem(int count);
}

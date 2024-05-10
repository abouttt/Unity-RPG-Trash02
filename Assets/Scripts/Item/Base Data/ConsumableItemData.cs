using UnityEngine;
using EnumType;

public abstract class ConsumableItemData : CountableItemData, ICooldownable
{
    [field: SerializeField]
    public int RequiredCount { get; private set; } = 1;

    public Cooldown Cooldown
    {
        get => _cooldown;
        set
        {
            return;
        }
    }

    [SerializeField]
    private Cooldown _cooldown;

    public ConsumableItemData()
    {
        ItemType = ItemType.Consumable;
    }

    public abstract override Item CreateItem();
    public abstract override CountableItem CreateItem(int count);
}

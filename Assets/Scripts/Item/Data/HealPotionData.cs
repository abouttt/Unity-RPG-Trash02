using UnityEngine;

[CreateAssetMenu(menuName = "Item/Consumable/HealPotion", fileName = "Item_Consumable_HealPotion")]
public class HealPotionData : ConsumableItemData
{
    [field: SerializeField]
    public int HealAmount { get; private set; }

    public override Item CreateItem()
    {
        return new HealPotion(this);
    }

    public override CountableItem CreateItem(int count)
    {
        return new HealPotion(this, count);
    }
}

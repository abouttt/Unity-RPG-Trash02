using UnityEngine;

[CreateAssetMenu(menuName = "Item/Consumable/ManaPotion", fileName = "Item_Consumable_ManaPotion")]
public class ManaPotionData : ConsumableItemData
{
    [field: SerializeField]
    public int ManaAmount { get; private set; }

    public override Item CreateItem()
    {
        return new ManaPotion(this);
    }

    public override CountableItem CreateItem(int count)
    {
        return new ManaPotion(this, count);
    }
}

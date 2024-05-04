using UnityEngine;

public class HealPotion : ConsumableItem
{
    private readonly HealPotionData _healPotionData;

    public HealPotion(HealPotionData data, int count = 1)
        : base(data, count)
    {
        _healPotionData = data;
    }

    public override bool Use()
    {
        if (!CheckCountAndSub())
        {
            return false;
        }

        Debug.Log($"Heal : {_healPotionData.HealAmount}");

        return true;
    }
}

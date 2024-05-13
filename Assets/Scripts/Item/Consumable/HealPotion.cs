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
        if (!CheckCanUseAndSubCount())
        {
            return false;
        }

        Player.Status.HP += _healPotionData.HealAmount;
        Managers.Resource.Instantiate("HealOnceBurst.prefab", Player.Collider.bounds.center, Player.Transform, true);

        return true;
    }
}

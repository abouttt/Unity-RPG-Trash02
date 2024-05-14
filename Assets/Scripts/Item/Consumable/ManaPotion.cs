using UnityEngine;

public class ManaPotion : ConsumableItem
{
    private readonly ManaPotionData _manaPotionData;

    public ManaPotion(ManaPotionData data, int count = 1)
        : base(data, count)
    {
        _manaPotionData = data;
    }

    public override bool Use()
    {
        if (!CheckCanUseAndSubCount())
        {
            return false;
        }

        Player.Status.MP += _manaPotionData.ManaAmount;
        Managers.Resource.Instantiate("ManaOnceBurst.prefab", Player.Collider.bounds.center, Player.Transform, true);

        return true;
    }
}

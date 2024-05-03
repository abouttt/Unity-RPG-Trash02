using UnityEngine;

public class ConsumableItem : CountableItem
{
    public ConsumableItemData ConsumableData { get; private set; }

    public ConsumableItem(ConsumableItemData data, int count = 1)
        : base(data, count)
    {
        ConsumableData = data;
    }

    protected bool CheckCanUseAndSubCount()
    {
        int remainingCount = CurrentCount - ConsumableData.RequiredCount;
        if (remainingCount < 0)
        {
            return false;
        }

        SetCount(remainingCount);

        return true;
    }
}

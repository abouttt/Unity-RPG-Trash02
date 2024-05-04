using UnityEngine;

public abstract class ConsumableItem : CountableItem, IUsable
{
    public ConsumableItemData ConsumableData { get; private set; }

    public ConsumableItem(ConsumableItemData data, int count = 1)
        : base(data, count)
    {
        ConsumableData = data;
    }

    public abstract bool Use();

    protected bool CheckCountAndSub()
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

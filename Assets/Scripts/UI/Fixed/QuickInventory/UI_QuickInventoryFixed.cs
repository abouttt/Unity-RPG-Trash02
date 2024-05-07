using UnityEngine;

public class UI_QuickInventoryFixed : UI_Base
{
    enum RectTransforms
    {
        QuickSlots,
    }

    private UI_QuickSlot[] _quickSlots;

    protected override void Init()
    {
        Managers.UI.Register<UI_QuickInventoryFixed>(this);

        BindRT(typeof(RectTransforms));

        Player.QuickInventory.InventoryChanged += index => _quickSlots[index].Refresh();

        for (int i = 0; i < Player.QuickInventory.Capacity; i++)
        {
            var go = Managers.Resource.Instantiate("UI_QuickSlot.prefab", GetRT((int)RectTransforms.QuickSlots));
            go.GetComponent<UI_QuickSlot>().Setup(i);
        }

        _quickSlots = GetRT((int)RectTransforms.QuickSlots).GetComponentsInChildren<UI_QuickSlot>();
    }
}

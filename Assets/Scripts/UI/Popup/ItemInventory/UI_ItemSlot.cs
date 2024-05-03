using UnityEngine;
using UnityEngine.EventSystems;
using EnumType;

public class UI_ItemSlot : UI_BaseSlot, IDropHandler
{
    enum Texts
    {
        CountText,
    }

    public ItemType ItemType { get; private set; }
    public int Index { get; private set; }

    protected override void Init()
    {
        base.Init();

        BindText(typeof(Texts));

        Clear();
    }

    public void Setup(ItemType itemType, int index)
    {
        ItemType = itemType;
        Index = index;
    }

    public void Refresh()
    {
        var item = Player.ItemInventory.GetItem<Item>(ItemType, Index);

        if (item != null)
        {
            if (ObjectRef != item)
            {
                if (HasObject)
                {
                    Clear();
                }

                SetObject(item, item.Data.ItemImage);

                if (item is CountableItem)
                {
                    item.ItemChanged += RefreshCountText;
                }

                RefreshCountText();
            }
        }
        else
        {
            Clear();
        }
    }

    protected override void Clear()
    {
        if (ObjectRef is Item item)
        {
            if (item is CountableItem)
            {
                item.ItemChanged -= RefreshCountText;
            }
        }

        base.Clear();
        GetText((int)Texts.CountText).gameObject.SetActive(false);
    }

    private void RefreshCountText()
    {
        if (ObjectRef is CountableItem countableItem && countableItem.CurrentCount > 1)
        {
            GetText((int)Texts.CountText).gameObject.SetActive(true);
            GetText((int)Texts.CountText).text = countableItem.CurrentCount.ToString();
        }
        else
        {
            GetText((int)Texts.CountText).gameObject.SetActive(false);
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {

    }

    public override void OnPointerExit(PointerEventData eventData)
    {

    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!CanPointerUp())
        {
            return;
        }

        var item = ObjectRef as Item;

        switch (ItemType)
        {
            default:
                break;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == gameObject)
        {
            return;
        }

        if (eventData.pointerDrag.TryGetComponent<UI_BaseSlot>(out var otherSlot))
        {
            switch (otherSlot.SlotType)
            {
                case SlotType.Item:
                    OnDropItemSlot(otherSlot as UI_ItemSlot);
                    break;
                default:
                    break;
            }
        }
    }

    private void OnDropItemSlot(UI_ItemSlot otherItemSlot)
    {
        var otherItem = otherItemSlot.ObjectRef as Item;

        if (!HasObject && otherItem is CountableItem otherCountableItem && otherCountableItem.CurrentCount > 1)
        {
            var splitPopup = Managers.UI.Show<UI_ItemSplitPopup>();
            splitPopup.SetEvent(() =>
                Player.ItemInventory.SplitItem(ItemType, otherItemSlot.Index, Index, splitPopup.CurrentCount),
                $"[{otherCountableItem.Data.ItemName}] 아이템 나누기", 1, otherCountableItem.CurrentCount);
        }
        else
        {
            Player.ItemInventory.MoveItem(ItemType, otherItemSlot.Index, Index);
        }
    }
}

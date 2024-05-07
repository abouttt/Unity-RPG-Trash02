using UnityEngine;
using UnityEngine.EventSystems;
using EnumType;

public class UI_QuickSlot : UI_BaseSlot, IDropHandler
{
    enum Texts
    {
        CountText,
        KeyInfoText,
    }

    enum CooldownImages
    {
        CooldownImage,
    }

    public int Index { get; private set; }

    protected override void Init()
    {
        base.Init();

        BindText(typeof(Texts));
        Bind<UI_CooldownImage>(typeof(CooldownImages));
    }

    private void Start()
    {
        Refresh();
    }

    public void Setup(int bindingIndex)
    {
        Index = bindingIndex;
        GetText((int)Texts.KeyInfoText).text = Managers.Input.GetBindingPath("Quick", bindingIndex);
    }

    public void Refresh()
    {
        var usable = Player.QuickInventory.GetUsable(Index);

        if (usable != null)
        {
            if (ObjectRef != usable)
            {
                if (HasObject)
                {
                    Clear();
                }

                if (usable is Item item)
                {
                    SetObject(usable, item.Data.ItemImage);

                    item.ItemChanged += IsItemDestroyed;

                    if (item is CountableItem)
                    {
                        item.ItemChanged += RefreshCountText;
                    }

                    if (item.Data is ICooldownable cooldownable)
                    {
                        Get<UI_CooldownImage>((int)CooldownImages.CooldownImage).SetCooldown(cooldownable.Cooldown);
                    }
                }
            }

            RefreshCountText();
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
            item.ItemChanged -= IsItemDestroyed;

            if (item is CountableItem)
            {
                item.ItemChanged -= RefreshCountText;
            }

            if (item.Data is ICooldownable)
            {
                Get<UI_CooldownImage>((int)CooldownImages.CooldownImage).Clear();
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

    private void IsItemDestroyed()
    {
        if (ObjectRef is Item item && item.IsDestroyed)
        {
            Player.QuickInventory.RemoveUsable(Index);
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        Managers.UI.Get<UI_TooltipTop>().ItemTooltip.SetSlot(this);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        Managers.UI.Get<UI_TooltipTop>().ItemTooltip.SetSlot(null);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!CanPointerUp())
        {
            return;
        }

        (ObjectRef as IUsable).Use();
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
                    if (otherSlot.ObjectRef is not IUsable usable)
                    {
                        return;
                    }

                    if (ObjectRef == usable)
                    {
                        return;
                    }

                    Player.QuickInventory.SetUsable(usable, Index);
                    break;
                case SlotType.Quick:
                    Player.QuickInventory.Swap(Index, (otherSlot as UI_QuickSlot).Index);
                    break;
            }
        }
    }
}

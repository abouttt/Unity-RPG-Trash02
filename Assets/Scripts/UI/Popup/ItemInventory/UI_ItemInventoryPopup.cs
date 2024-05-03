using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EnumType;

public class UI_ItemInventoryPopup : UI_Popup
{
    enum RectTransforms
    {
        EquipmentSlots,
        ConsumableSlots,
        EtcSlots
    }

    enum Texts
    {
        GoldText
    }

    enum Buttons
    {
        CloseButton,
    }

    enum ScrollRects
    {
        ItemSlotScrollView,
    }

    enum Tabs
    {
        EquipmentTabButton,
        ConsumableTabButton,
        EtcTabButton,
    }

    [SerializeField]
    private float _tabXOffset;

    [SerializeField]
    private float _tabClickedXPosition;

    private readonly Dictionary<ItemType, UI_ItemSlot[]> _slots = new();
    private readonly Dictionary<UI_ItemInventoryTab, RectTransform> _tabs = new();

    protected override void Init()
    {
        base.Init();

        BindRT(typeof(RectTransforms));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        Bind<ScrollRect>(typeof(ScrollRects));
        Bind<UI_ItemInventoryTab>(typeof(Tabs));

        Player.ItemInventory.InventoryChanged += (itemType, index) => _slots[itemType][index].Refresh();

        GetButton((int)Buttons.CloseButton).onClick.AddListener(Managers.UI.Close<UI_ItemInventoryPopup>);

        InitSlots();
        InitTabs();
    }

    private void Start()
    {
        Managers.UI.Register<UI_ItemInventoryPopup>(this);

        Showed += () =>
        {
            PopupRT.SetParent(transform);
            Get<ScrollRect>((int)ScrollRects.ItemSlotScrollView).verticalScrollbar.value = 1f;
        };

        ShowItemSlots(ItemType.Equipment);
    }

    public void ShowItemSlots(ItemType itemType)
    {
        var scrollView = Get<ScrollRect>((int)ScrollRects.ItemSlotScrollView);
        scrollView.verticalScrollbar.value = 1f;

        foreach (var tab in _tabs)
        {
            var pos = tab.Key.SlotsRT.anchoredPosition;

            if (tab.Key.TabType == itemType)
            {
                scrollView.content = tab.Value;
                pos.x = _tabClickedXPosition;
                tab.Value.gameObject.SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(tab.Value);
            }
            else
            {
                pos.x = _tabXOffset;
                tab.Value.gameObject.SetActive(false);
            }

            tab.Key.SlotsRT.anchoredPosition = pos;
        }
    }

    private void InitSlots()
    {
        var itemInventory = Player.ItemInventory;
        CreateSlots(itemInventory.Inventories[ItemType.Equipment].Capacity, ItemType.Equipment, GetRT((int)RectTransforms.EquipmentSlots));
        CreateSlots(itemInventory.Inventories[ItemType.Consumable].Capacity, ItemType.Consumable, GetRT((int)RectTransforms.ConsumableSlots));
        CreateSlots(itemInventory.Inventories[ItemType.Etc].Capacity, ItemType.Etc, GetRT((int)RectTransforms.EtcSlots));

        _slots.Add(ItemType.Equipment, GetRT((int)RectTransforms.EquipmentSlots).GetComponentsInChildren<UI_ItemSlot>());
        _slots.Add(ItemType.Consumable, GetRT((int)RectTransforms.ConsumableSlots).GetComponentsInChildren<UI_ItemSlot>());
        _slots.Add(ItemType.Etc, GetRT((int)RectTransforms.EtcSlots).GetComponentsInChildren<UI_ItemSlot>());
    }

    private void InitTabs()
    {
        _tabs.Add(Get<UI_ItemInventoryTab>((int)Tabs.EquipmentTabButton), GetRT((int)RectTransforms.EquipmentSlots));
        _tabs.Add(Get<UI_ItemInventoryTab>((int)Tabs.ConsumableTabButton), GetRT((int)RectTransforms.ConsumableSlots));
        _tabs.Add(Get<UI_ItemInventoryTab>((int)Tabs.EtcTabButton), GetRT((int)RectTransforms.EtcSlots));

        foreach (var tab in _tabs)
        {
            tab.Key.GetComponent<Button>().onClick.AddListener(() =>
            {
                SetTop();
                ShowItemSlots(tab.Key.TabType);
            });
        }
    }

    private void CreateSlots(int capacity, ItemType itemType, Transform parent)
    {
        for (int i = 0; i < capacity; i++)
        {
            var go = Managers.Resource.Instantiate("UI_ItemSlot.prefab", parent);
            go.GetComponent<UI_ItemSlot>().Setup(itemType, i);
        }
    }
}

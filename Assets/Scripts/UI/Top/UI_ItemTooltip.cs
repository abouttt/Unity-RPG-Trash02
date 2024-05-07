using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using EnumType;

public class UI_ItemTooltip : UI_Base
{
    enum GameObjects
    {
        Tooltip,
    }

    enum Texts
    {
        ItemNameText,
        ItemTypeText,
        ItemDescText,
    }

    [SerializeField]
    [Tooltip("Distance from mouse")]
    private Vector2 _offset;

    [Space(10)]
    [SerializeField]
    private Color _lowColor = Color.white;

    [SerializeField]
    private Color _normalColor = Color.white;

    [SerializeField]
    private Color _highColor = Color.white;

    private UI_BaseSlot _slot;
    private Item _itemRef;
    private ItemData _itemDataRef;
    private RectTransform _rt;
    private readonly StringBuilder _sb = new(50);

    protected override void Init()
    {
        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));

        _rt = GetObject((int)GameObjects.Tooltip).GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        GetObject((int)GameObjects.Tooltip).SetActive(false);
    }

    private void Update()
    {
        if (_slot == null)
        {
            gameObject.SetActive(false);
            return;
        }

        if (!_slot.gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
            return;
        }

        if (UI_BaseSlot.IsDragging)
        {
            GetObject((int)GameObjects.Tooltip).SetActive(false);
            return;
        }

        if (_slot.HasObject)
        {
            if (_slot.ObjectRef is Item item)
            {
                SetItemData(item.Data);
            }
            else if (_slot.ObjectRef is ItemData itemData)
            {
                SetItemData(itemData);
            }
        }
        else
        {
            GetObject((int)GameObjects.Tooltip).SetActive(false);
        }

        SetPosition(Mouse.current.position.ReadValue());
    }

    public void SetSlot(UI_BaseSlot slot)
    {
        _slot = slot;
        gameObject.SetActive(slot != null);
    }

    private void SetItemData(ItemData itemData)
    {
        GetObject((int)GameObjects.Tooltip).SetActive(true);

        if (_itemDataRef != null && _itemDataRef.Equals(itemData))
        {
            return;
        }

        _itemDataRef = itemData;
        GetText((int)Texts.ItemNameText).text = itemData.ItemName;
        SetItemQualityColor(itemData.ItemQuality);
        SetType(itemData.ItemType);
        SetDescription(itemData);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rt);
    }

    private void SetItemQualityColor(ItemQuality itemQuality)
    {
        GetText((int)Texts.ItemNameText).color = itemQuality switch
        {
            ItemQuality.Low => _lowColor,
            ItemQuality.Normal => _normalColor,
            ItemQuality.High => _highColor,
            _ => Color.red,
        };
    }

    private void SetType(ItemType itemType)
    {
        GetText((int)Texts.ItemTypeText).text = itemType switch
        {
            ItemType.Equipment => "[장비 아이템]",
            ItemType.Consumable => "[소비 아이템]",
            ItemType.Etc => "[기타 아이템]",
            _ => "[NULL]"
        };
    }

    private void SetDescription(ItemData itemData)
    {
        _sb.Clear();

        if (itemData is not EtcItemData)
        {
            _sb.Append($"제한 레벨 : {itemData.LimitLevel} \n");
        }

        if (itemData is EquipmentItemData equipmentItemData)
        {
            _sb.Append("\n");
            AppendValueIfGreaterThan0("공격력", equipmentItemData.Damage);
            AppendValueIfGreaterThan0("방어력", equipmentItemData.Defense);
            AppendValueIfGreaterThan0("체력", equipmentItemData.HP);
            AppendValueIfGreaterThan0("마나", equipmentItemData.MP);
            AppendValueIfGreaterThan0("기력", equipmentItemData.SP);
        }
        else if (itemData is ConsumableItemData consumableItemData)
        {
            _sb.Append($"소비 개수 : {consumableItemData.RequiredCount}\n");
        }

        if (_sb.Length > 0)
        {
            _sb.Append("\n");
        }

        if (!string.IsNullOrEmpty(itemData.Description))
        {
            _sb.Append($"{itemData.Description}\n\n");
        }

        GetText((int)Texts.ItemDescText).text = _sb.ToString();
    }

    private void AppendValueIfGreaterThan0(string text, int value)
    {
        if (value > 0)
        {
            _sb.Append($"{text} +{value}\n");
        }
    }

    private void SetPosition(Vector3 position)
    {
        var nextPosition = new Vector3()
        {
            x = position.x + (_rt.rect.width * 0.5f) + _offset.x,
            y = position.y + (_rt.rect.height * 0.5f) + _offset.y
        };

        _rt.position = nextPosition;
    }
}

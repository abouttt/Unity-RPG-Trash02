using UnityEngine;
using UnityEngine.UI;
using EnumType;

public class UI_ItemTooltip : UI_BaseTooltip
{
    enum Texts
    {
        ItemNameText,
        ItemTypeText,
        ItemDescText,
    }

    [Space(10)]
    [SerializeField]
    private Color _lowColor = Color.white;

    [SerializeField]
    private Color _normalColor = Color.white;

    [SerializeField]
    private Color _highColor = Color.white;

    protected override void Init()
    {
        base.Init();

        BindText(typeof(Texts));
    }

    protected override void SetData()
    {
        if (SlotRef.ObjectRef is Item item)
        {
            SetItemData(item.Data);
        }
        else if (SlotRef.ObjectRef is ItemData itemData)
        {
            SetItemData(itemData);
        }
    }

    private void SetItemData(ItemData itemData)
    {
        GetObject((int)GameObjects.Tooltip).SetActive(true);

        if (DataRef != null && DataRef.Equals(itemData))
        {
            return;
        }

        DataRef = itemData;
        GetText((int)Texts.ItemNameText).text = itemData.ItemName;
        SetItemQualityColor(itemData.ItemQuality);
        SetType(itemData.ItemType);
        SetDescription(itemData);
        LayoutRebuilder.ForceRebuildLayoutImmediate(RT);
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
            ItemType.Equipment => "[��� ������]",
            ItemType.Consumable => "[�Һ� ������]",
            ItemType.Etc => "[��Ÿ ������]",
            _ => "[NULL]"
        };
    }

    private void SetDescription(ItemData itemData)
    {
        SB.Clear();

        if (itemData is not EtcItemData)
        {
            SB.Append($"���� ���� : {itemData.LimitLevel} \n");
        }

        if (itemData is EquipmentItemData equipmentItemData)
        {
            SB.Append("\n");
            AppendValueIfGreaterThan0("���ݷ�", equipmentItemData.Damage);
            AppendValueIfGreaterThan0("����", equipmentItemData.Defense);
            AppendValueIfGreaterThan0("ü��", equipmentItemData.HP);
            AppendValueIfGreaterThan0("����", equipmentItemData.MP);
            AppendValueIfGreaterThan0("���", equipmentItemData.SP);
        }
        else if (itemData is ConsumableItemData consumableItemData)
        {
            SB.Append($"�Һ� ���� : {consumableItemData.RequiredCount}\n");
        }

        if (SB.Length > 0)
        {
            SB.Append("\n");
        }

        if (!string.IsNullOrEmpty(itemData.Description))
        {
            SB.Append($"{itemData.Description}\n\n");
        }

        GetText((int)Texts.ItemDescText).text = SB.ToString();
    }

    private void AppendValueIfGreaterThan0(string text, int value)
    {
        if (value > 0)
        {
            SB.Append($"{text} +{value}\n");
        }
    }
}

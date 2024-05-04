using System;
using System.Collections.Generic;
using UnityEngine;
using EnumType;

public class EquipmentInventory : BaseMonoBehaviour
{
    public event Action<EquipmentType> InventoryChanged;

    private readonly Dictionary<EquipmentType, EquipmentItem> _items = new();

    private void Awake()
    {
        var types = Enum.GetValues(typeof(EquipmentType));
        for (int i = 0; i < types.Length; i++)
        {
            _items.Add((EquipmentType)types.GetValue(i), null);
        }
    }

    public void Equip(EquipmentItemData equipmentItemData)
    {
        var equipmentItem = equipmentItemData.CreateItem() as EquipmentItem;
        var equipmentType = equipmentItemData.EquipmentType;
        _items[equipmentType]?.Destroy();
        _items[equipmentType] = equipmentItem;
        InventoryChanged?.Invoke(equipmentType);
    }

    public void Unequip(EquipmentType equipmentType)
    {
        _items[equipmentType]?.Destroy();
        _items[equipmentType] = null;
        InventoryChanged?.Invoke(equipmentType);
    }

    public EquipmentItem GetItem(EquipmentType equipmentType)
    {
        return _items[equipmentType];
    }

    public bool IsEquipped(EquipmentType equipmentType)
    {
        return _items[equipmentType] != null;
    }
}

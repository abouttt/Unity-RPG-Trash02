using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using EnumType;

public class ItemInventory : MonoBehaviour
{
    [Serializable]
    public class Inventory
    {
        public List<Item> Items;
        public int Capacity;
        [ReadOnly]
        public int Count;
    }

    public event Action<ItemType, int> InventoryChanged;

    public IReadOnlyDictionary<ItemType, Inventory> Inventories => _inventories;

    [SerializeField]
    private SerializedDictionary<ItemType, Inventory> _inventories;
    private readonly Dictionary<Item, int> _itemIndexes = new();

    private void Awake()
    {
        foreach (var element in _inventories)
        {
            element.Value.Items = Enumerable.Repeat<Item>(null, element.Value.Capacity).ToList();
        }
    }

    public int AddItem(ItemData itemData, int count = 1)
    {
        var inventory = _inventories[itemData.ItemType];
        var countableItemData = itemData as CountableItemData;

        while (count > 0)
        {
            if (countableItemData != null)
            {
                int sameItemIndex = inventory.Items.FindIndex(item =>
                {
                    if (item == null)
                    {
                        return false;
                    }

                    if (!item.Data.Equals(countableItemData))
                    {
                        return false;
                    }

                    return !(item as CountableItem).IsMax;
                });

                if (sameItemIndex != -1)
                {
                    var otherCountableItem = inventory.Items[sameItemIndex] as CountableItem;
                    count = otherCountableItem.AddCountAndGetExcess(count);
                }
                else
                {
                    if (TryGetEmptyIndex(itemData.ItemType, out var emptyIndex))
                    {
                        SetItem(countableItemData, emptyIndex, count);
                        count = Mathf.Max(0, count - countableItemData.MaxCount);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                if (TryGetEmptyIndex(itemData.ItemType, out var emptyIndex))
                {
                    SetItem(itemData, emptyIndex);
                    count--;
                }
                else
                {
                    break;
                }
            }
        }

        return count;
    }

    public void RemoveItem(Item item)
    {
        if (item == null)
        {
            return;
        }

        if (item.IsDestroyed)
        {
            return;
        }

        int index = _itemIndexes[item];
        DestroyItem(item.Data.ItemType, index);
        InventoryChanged?.Invoke(item.Data.ItemType, index);
    }

    public void RemoveItem(ItemType itemType, int index)
    {
        RemoveItem(_inventories[itemType].Items[index]);
    }

    public void MoveItem(ItemType itemType, int fromIndex, int toIndex)
    {
        if (fromIndex == toIndex)
        {
            return;
        }

        if (!AddAllCountFromTo(itemType, fromIndex, toIndex))
        {
            SwapItem(itemType, fromIndex, toIndex);
        }
    }

    public void SplitItem(ItemType itemType, int fromIndex, int toIndex, int count)
    {
        if (fromIndex == toIndex)
        {
            return;
        }

        if (IsEmpty(itemType, fromIndex) || !IsEmpty(itemType, toIndex))
        {
            return;
        }

        var inventory = _inventories[itemType];

        if (inventory.Items[fromIndex] is not CountableItem fromItem)
        {
            return;
        }

        int remainingCount = fromItem.CurrentCount - count;
        if (remainingCount < 0)
        {
            return;
        }
        else if (remainingCount == 0)
        {
            SwapItem(itemType, fromIndex, toIndex);
        }
        else
        {
            fromItem.SetCount(remainingCount);
            SetItem(fromItem.CountableData, toIndex, count);
        }
    }

    public void SetItem(ItemData itemData, int index, int count = 1)
    {
        if (itemData == null)
        {
            return;
        }

        var inventory = _inventories[itemData.ItemType];

        if (IsEmpty(itemData.ItemType, index))
        {
            inventory.Count++;
        }
        else
        {
            DestroyItem(itemData.ItemType, index);
        }

        if (itemData is CountableItemData countableItemData)
        {
            inventory.Items[index] = countableItemData.CreateItem(count);
        }
        else
        {
            inventory.Items[index] = itemData.CreateItem();
        }

        _itemIndexes.Add(inventory.Items[index], index);
        InventoryChanged?.Invoke(itemData.ItemType, index);
    }

    public T GetItem<T>(ItemType itemType, int index) where T : Item
    {
        return _inventories[itemType].Items[index] as T;
    }

    public int GetItemIndex(Item item)
    {
        if (_itemIndexes.TryGetValue(item, out var index))
        {
            return index;
        }

        return -1;
    }

    public bool IsEmpty(ItemType itemType, int index)
    {
        return _inventories[itemType].Items[index] == null;
    }

    private void SwapItem(ItemType itemType, int Aindex, int BIndex)
    {
        var items = _inventories[itemType].Items;

        if (!IsEmpty(itemType, Aindex))
        {
            _itemIndexes[items[Aindex]] = BIndex;
        }

        if (!IsEmpty(itemType, BIndex))
        {
            _itemIndexes[items[BIndex]] = Aindex;
        }

        (items[Aindex], items[BIndex]) = (items[BIndex], items[Aindex]);

        InventoryChanged?.Invoke(itemType, Aindex);
        InventoryChanged?.Invoke(itemType, BIndex);
    }

    private bool AddAllCountFromTo(ItemType itemType, int fromIndex, int toIndex)
    {
        var inventory = _inventories[itemType];

        if (inventory.Items[fromIndex] is not CountableItem fromItem ||
            inventory.Items[toIndex] is not CountableItem toItem)
        {
            return false;
        }

        if (!fromItem.Data.Equals(toItem.Data))
        {
            return false;
        }

        if (toItem.IsMax)
        {
            return false;
        }

        toItem.AddCountAndGetExcess(fromItem.CurrentCount);
        DestroyItem(itemType, fromIndex);
        InventoryChanged?.Invoke(itemType, fromIndex);

        return true;
    }

    private bool TryGetEmptyIndex(ItemType itemType, out int index)
    {
        index = _inventories[itemType].Items.FindIndex(item => item == null);
        return index != -1;
    }

    private void DestroyItem(ItemType itemType, int index)
    {
        var inventory = _inventories[itemType];
        var item = inventory.Items[index];
        inventory.Items[index] = null;
        inventory.Count--;
        item.Destroy();
        _itemIndexes.Remove(item);
    }
}

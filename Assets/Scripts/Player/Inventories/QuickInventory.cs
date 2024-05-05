using System;
using System.Collections.Generic;
using UnityEngine;

public class QuickInventory : BaseMonoBehaviour
{
    public event Action<int, IUsable> InventoryChanged;

    [field: SerializeField]
    public int Capacity { get; private set; }

    private readonly Dictionary<int, IUsable> _usables = new();

    private void Awake()
    {
        for (int i = 0; i < Capacity; i++)
        {
            _usables.Add(i, null);
        }
    }

    public void SetUsable(IUsable usable, int index)
    {
        if (usable == null)
        {
            return;
        }

        _usables[index] = usable;
        InventoryChanged?.Invoke(index, usable);
    }

    public void RemoveUsable(int index)
    {
        if (_usables[index] == null)
        {
            return;
        }

        _usables[index] = null;
        InventoryChanged?.Invoke(index, null);
    }

    public IUsable GetUsable(int index)
    {
        return _usables[index];
    }

    public void Swap(int indexA, int indexB)
    {
        var usableA = _usables[indexA];
        var usableB = _usables[indexB];
        SetUsable(usableA, indexB);
        SetUsable(usableB, indexA);
    }
}

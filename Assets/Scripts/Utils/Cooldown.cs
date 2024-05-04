using System;
using UnityEngine;

[Serializable]
public class Cooldown
{
    public event Action Cooldowned;

    [field: SerializeField]
    public float Max { get; private set; }
    public float Current;

    public void OnCooldowned()
    {
        Current = Max;
        Cooldowned?.Invoke();
    }

    public void Clear()
    {
        Current = 0;
        Cooldowned = null;
    }
}

using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool Enabled
    {
        get => _collider.enabled;
        set
        {
            _collider.enabled = value;
            if (!value)
            {
                _hittedColliders.Clear();
            }
        }
    }

    private Collider _collider;
    private readonly HashSet<Collider> _hittedColliders = new();

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Weapon");
        _collider = GetComponent<Collider>();
        _collider.isTrigger = true;
        _collider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hittedColliders.Contains(other))
        {
            return;
        }

        var monster = other.GetComponent<Monster>();
        monster.TakeDamage(Player.Status.Damage);
        _hittedColliders.Add(other);
    }
}

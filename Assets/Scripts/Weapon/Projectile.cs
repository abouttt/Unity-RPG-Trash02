using UnityEngine;

public class Projectile : BaseMonoBehaviour
{
    public int Damage { get; set; }

    [Header("Owner")]
    [SerializeField]
    private bool _player;

    [SerializeField]
    private bool _enemy;

    [SerializeField]
    private string _hitEffectPrefabAddresse;

    private Rigidbody _rb;
    private bool _canDestroy;

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Projectile");
        _rb = GetComponent<Rigidbody>();
    }

    public void Shoot(Vector3 force)
    {
        _rb.velocity = Vector3.zero;
        _rb.AddForce(force, ForceMode.VelocityChange);
    }

    private void OnTriggerEnter(Collider other)
    {
        _canDestroy = true;
        int otherLayer = other.gameObject.layer;

        if (_player)
        {
            if (otherLayer == LayerMask.NameToLayer("Enemy"))
            {
                other.GetComponent<Monster>().TakeDamage(Damage);
            }
            else if (otherLayer == LayerMask.NameToLayer("Player"))
            {
                _canDestroy = false;
            }
        }
        else if (_enemy)
        {
            if (otherLayer == LayerMask.NameToLayer("Player"))
            {
                if (Player.Movement.IsRolling)
                {
                    return;
                }

                Player.Combat.TakeDamage(null, other.ClosestPoint(transform.position), Damage, false);
            }
            else if (otherLayer == LayerMask.NameToLayer("Shield"))
            {
                if (!Player.Combat.IsDefending)
                {
                    return;
                }

                Player.Combat.HitShield(other.ClosestPoint(transform.position));
            }
            else if (otherLayer == LayerMask.NameToLayer("Enemy"))
            {
                _canDestroy = false;
            }
        }

        if (otherLayer == LayerMask.NameToLayer("Environment"))
        {
            Managers.Resource.Instantiate(_hitEffectPrefabAddresse, other.ClosestPoint(transform.position), null, true);
        }

        if (_canDestroy)
        {
            Managers.Resource.Destroy(gameObject);
        }
    }
}

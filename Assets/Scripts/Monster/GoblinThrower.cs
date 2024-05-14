using UnityEngine;
using EnumType;

public class GoblinThrower : Monster
{
    [SerializeField]
    private Transform _rightHand;

    [SerializeField]
    private float _projectileSpeed;

    private void OnAttack()
    {
        if (CurrentState != MonsterState.Attack)
        {
            return;
        }

        int cnt = Physics.OverlapSphereNonAlloc(AttackOffset.position, AttackRadius, PlayerCollider, 1 << LayerMask.NameToLayer("Player"));
        if (cnt != 0)
        {
            Player.Combat.TakeDamage(this, transform.position, Data.Damage, false);
        }
        else
        {
            var go = Managers.Resource.Instantiate(
                "GoblinThrowerThrowingWeapon.prefab", _rightHand.position, transform.rotation * Quaternion.Euler(90f, 0f, 0f), null, true);
            var projectile = go.GetComponent<Projectile>();
            projectile.Damage = Data.Damage;
            projectile.Shoot(transform.forward * _projectileSpeed);
        }
    }
}

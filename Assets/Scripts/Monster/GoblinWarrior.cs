using UnityEngine;
using EnumType;

public class GoblinWarrior : Monster
{
    private void OnAttack()
    {
        if (CurrentState != MonsterState.Attack)
        {
            return;
        }

        int cnt = Physics.OverlapSphereNonAlloc(AttackOffset.position, AttackRadius, PlayerCollider, 1 << LayerMask.NameToLayer("Player"));
        if (cnt != 0)
        {
            Player.Combat.TakeDamage(this, transform.position, Data.Damage, true);
        }
    }
}

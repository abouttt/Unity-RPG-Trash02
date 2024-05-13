using UnityEngine;
using EnumType;

public class Monster_CombatIdleState : StateMachineBehaviour
{
    private Monster _monster;
    private float _currentAttackDelayTime;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_monster == null)
        {
            _monster = animator.GetComponent<Monster>();
        }

        _monster.SetActiveNaveMeshAgentUpdate(false);
        _monster.Animator.SetBool(_monster.AnimIDAttack, false);
        _currentAttackDelayTime = 0f;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_monster.CurrentState != MonsterState.Attack)
        {
            return;
        }

        if (Vector3.Distance(Player.Transform.position, _monster.transform.position) <= _monster.AttackDistance)
        {
            _currentAttackDelayTime += Time.deltaTime;
            if (_currentAttackDelayTime >= _monster.AttackDelayTime)
            {
                _monster.Transition(MonsterState.Attack);
            }
        }
        else
        {
            _monster.Transition(MonsterState.Tracking);
        }
    }
}

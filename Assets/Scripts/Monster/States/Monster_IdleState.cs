using UnityEngine;
using EnumType;

public class Monster_IdleState : StateMachineBehaviour
{
    private Monster _monster;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_monster == null)
        {
            _monster = animator.GetComponent<Monster>();
        }

        _monster.ResetAllTriggers();
        _monster.SetActiveNaveMeshAgentUpdate(false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_monster.CurrentState != MonsterState.Idle)
        {
            return;
        }

        _monster.Fov.CheckFieldOfView();

        if (_monster.Fov.IsDetected)
        {
            if (Vector3.Distance(Player.Transform.position, _monster.transform.position) <= _monster.AttackDistance)
            {
                _monster.Transition(MonsterState.Attack);
            }
            else
            {
                _monster.Transition(MonsterState.Tracking);
            }
        }
    }
}

using UnityEngine;
using EnumType;

public class Monster_TrackingState : StateMachineBehaviour
{
    private Monster _monster;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_monster == null)
        {
            _monster = animator.GetComponent<Monster>();
        }

        _monster.SetActiveNaveMeshAgentUpdate(true);
        _monster.Animator.SetBool(_monster.AnimIDTracking, false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_monster.CurrentState != MonsterState.Tracking)
        {
            return;
        }

        _monster.Rotation(_monster.NavMeshAgent.steeringTarget);

        var dist = Vector3.Distance(Player.Transform.position, _monster.transform.position);
        if (dist > _monster.TrackingDistance)
        {
            _monster.Transition(MonsterState.Restore);
        }
        else if (dist <= _monster.AttackDistance)
        {
            _monster.Transition(MonsterState.Attack);
        }
        else
        {
            _monster.NavMeshAgent.SetDestination(Player.Transform.position);
        }
    }
}

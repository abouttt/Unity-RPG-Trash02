using UnityEngine;
using EnumType;

public class Monster_RestoreState : StateMachineBehaviour
{
    private Monster _monster;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_monster == null)
        {
            _monster = animator.GetComponent<Monster>();
        }

        _monster.ResetAllTriggers();
        _monster.SetActiveNaveMeshAgentUpdate(true);
        _monster.NavMeshAgent.SetDestination(_monster.OriginalPosition);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_monster.CurrentState != MonsterState.Restore)
        {
            return;
        }

        _monster.Rotation(_monster.NavMeshAgent.steeringTarget);
        _monster.Fov.CheckFieldOfView();

        if (_monster.Fov.IsDetected)
        {
            _monster.Transition(MonsterState.Tracking);
        }
        else if (!_monster.NavMeshAgent.pathPending && _monster.NavMeshAgent.remainingDistance <= 0f)
        {
            _monster.Transition(MonsterState.Idle);
        }
    }
}

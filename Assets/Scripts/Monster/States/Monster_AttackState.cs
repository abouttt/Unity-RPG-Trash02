using UnityEngine;
using EnumType;

public class Monster_AttackState : StateMachineBehaviour
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
        if (_monster.CurrentState != MonsterState.Attack)
        {
            return;
        }

        if (stateInfo.normalizedTime < 0.3f)
        {
            _monster.Rotation(Player.Transform.position);
        }
    }
}

using UnityEngine;
using EnumType;

public class Monster_StunnedState : StateMachineBehaviour
{
    private Monster _monster;
    private GameObject _stunnedEffect;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_monster == null)
        {
            _monster = animator.GetComponent<Monster>();
        }

        _monster.ResetAllTriggers();
        _monster.SetActiveNaveMeshAgentUpdate(false);

        var bounds = _monster.Collider.bounds;
        _stunnedEffect = Managers.Resource.Instantiate("Stunned.prefab", bounds.center + new Vector3(0f, bounds.extents.y, 0f), null, true);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_monster.CurrentState != MonsterState.Stunned)
        {
            return;
        }

        if (stateInfo.normalizedTime >= 1f)
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

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Managers.Resource.Destroy(_stunnedEffect);
    }
}

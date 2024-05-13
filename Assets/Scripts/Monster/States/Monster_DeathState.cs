using UnityEngine;
using EnumType;

public class Monster_DeathState : StateMachineBehaviour
{
    private Monster _monster;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_monster == null)
        {
            _monster = animator.GetComponent<Monster>();
        }

        _monster.ResetAllTriggers();
        _monster.SetActiveNaveMeshAgentUpdate(false);
        _monster.Collider.isTrigger = true;

        if (_monster.IsLockOnTarget)
        {
            Player.Camera.LockedTarget = null;
        }

        foreach (var collider in _monster.LockOnTargetColliders)
        {
            collider.enabled = false;
        }

        Player.Status.XP += _monster.Data.GetXP();
        Player.Status.Gold += _monster.Data.GetGold();
        Managers.Quest.ReceiveReport(Category.Monster, _monster.Data.MonsterID, 1);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_monster.CurrentState != MonsterState.Death)
        {
            return;
        }

        if (stateInfo.normalizedTime >= 1f)
        {
            _monster.Data.DropItems(_monster.transform.position);
            Managers.Resource.Destroy(_monster.gameObject);
        }
    }
}

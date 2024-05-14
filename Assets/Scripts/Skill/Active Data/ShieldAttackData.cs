using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Active/ShieldAttack", fileName = "Skill_Active_ShieldAttack")]
public class ShieldAttackData : ActiveSkillData
{
    public override Skill CreateSkill(int level)
    {
        return new ShieldAttack(this, level);
    }
}

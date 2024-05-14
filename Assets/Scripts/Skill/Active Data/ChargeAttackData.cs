using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Active/ChargeAttack", fileName = "Skill_Active_ChargeAttack")]
public class ChargeAttackData : ActiveSkillData
{
    public override Skill CreateSkill(int level)
    {
        return new ChargeAttack(this, level);
    }
}

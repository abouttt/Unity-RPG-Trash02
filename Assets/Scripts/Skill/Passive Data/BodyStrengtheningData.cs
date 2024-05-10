using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Passive/BodyStrengthening", fileName = "Skill_Passive_BodyStrengthening")]
public class BodyStrengtheningData : PassiveSkillData
{
    public override Skill CreateSkill()
    {
        return new BodyStrengthening(this);
    }
}

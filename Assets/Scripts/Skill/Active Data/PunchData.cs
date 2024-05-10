using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Active/Punch", fileName = "Skill_Active_Punch")]
public class PunchData : ActiveSkillData
{
    public override Skill CreateSkill()
    {
        return new Punch(this);
    }
}

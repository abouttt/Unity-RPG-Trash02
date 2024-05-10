using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Active/Kick", fileName = "Skill_Active_Kick")]
public class KickData : ActiveSkillData
{
    public override Skill CreateSkill()
    {
        return new Kick(this);
    }
}

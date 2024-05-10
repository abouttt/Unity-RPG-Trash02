using UnityEngine;
using EnumType;

public abstract class PassiveSkillData : SkillData
{
    public PassiveSkillData()
    {
        SkillType = SkillType.Passive;
    }
}

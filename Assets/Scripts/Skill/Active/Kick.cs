using UnityEngine;

public class Kick : ActiveSkill
{
    public Kick(ActiveSkillData data, int level = 0)
        : base(data, level)
    { }

    public override bool Use()
    {
        if (!CheckCanuseAndSub())
        {
            return false;
        }

        Debug.Log("Kick Skill");
        return true;
    }

    protected override void RefreshStatDescription()
    {

    }
}

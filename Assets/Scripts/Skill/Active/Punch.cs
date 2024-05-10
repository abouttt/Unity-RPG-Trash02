using UnityEngine;

public class Punch : ActiveSkill
{
    public Punch(ActiveSkillData data, int level = 0)
        : base(data, level)
    { }

    public override bool Use()
    {
        if (!CheckCanuseAndSub())
        {
            return false;
        }

        Debug.Log("Kick Punch");
        return true;
    }

    protected override void RefreshStatDescription()
    {

    }
}

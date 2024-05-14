using UnityEngine;

public abstract class ActiveSkill : Skill, IUsable
{
    public ActiveSkillData ActiveData { get; private set; }

    public ActiveSkill(ActiveSkillData data, int level)
        : base(data, level)
    {
        ActiveData = data;
    }

    public abstract bool Use();

    public void SubRequired()
    {
        Player.Status.HP -= ActiveData.RequiredHP;
        Player.Status.MP -= ActiveData.RequiredMP;
        Player.Status.SP -= ActiveData.RequiredSP;

        ActiveData.Cooldown.OnCooldowned();
        Managers.Cooldown.AddCooldown(ActiveData.Cooldown);
    }

    protected virtual bool CheckCanUse()
    {
        if (ActiveData.Cooldown.Current > 0f)
        {
            return false;
        }

        if (Player.Status.HP < 0 ||
            Player.Status.MP < 0 ||
            Player.Status.SP < 0f)
        {
            return false;
        }

        return true;
    }
}

using UnityEngine;

public abstract class PassiveSkill : Skill
{
    public PassiveSkillData PassiveData { get; private set; }

    public PassiveSkill(PassiveSkillData data, int level)
        : base(data, level)
    {
        PassiveData = data;

        if (level > 0)
        {
            if (Data.PerStats.Length > 0)
            {
                Player.Status.ExtraPerStat += Data.PerStats[CurrentLevel - 1];
            }

            if (Data.FixedStats.Length > 0)
            {
                Player.Status.ExtraFixedStat += Data.FixedStats[CurrentLevel - 1];
            }
        }
    }

    public override void LevelUp()
    {
        base.LevelUp();

        if (CurrentLevel > 1)
        {
            if (Data.PerStats.Length > 0)
            {
                Player.Status.ExtraPerStat += Data.PerStats[CurrentLevel - 1] - Data.PerStats[CurrentLevel - 2];
            }

            if (Data.FixedStats.Length > 0)
            {
                Player.Status.ExtraFixedStat += Data.FixedStats[CurrentLevel - 1] - Data.FixedStats[CurrentLevel - 2];
            }
        }
        else
        {
            if (Data.PerStats.Length > 0)
            {
                Player.Status.ExtraPerStat += Data.PerStats[CurrentLevel - 1];
            }

            if (Data.FixedStats.Length > 0)
            {
                Player.Status.ExtraFixedStat += Data.FixedStats[CurrentLevel - 1];
            }
        }
    }

    public override int ResetSkill()
    {
        if (IsUnlocked)
        {
            if (Data.PerStats.Length > 0)
            {
                Player.Status.ExtraPerStat -= Data.PerStats[CurrentLevel - 1];
            }

            if (Data.FixedStats.Length > 0)
            {
                Player.Status.ExtraFixedStat -= Data.FixedStats[CurrentLevel - 1];
            }
        }

        return base.ResetSkill();
    }
}

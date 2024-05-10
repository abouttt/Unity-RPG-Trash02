using UnityEngine;

public abstract class PassiveSkill : Skill
{
    public PassiveSkillData PassiveData { get; private set; }

    private int _currentStatIndex = -1;

    public PassiveSkill(PassiveSkillData data, int level = 0)
        : base(data, level)
    {
        PassiveData = data;
        SkillChanged += RefreshStats;
    }

    public void RefreshStats()
    {
        if (!IsUnlocked)
        {
            return;
        }

        if (CurrentLevel == 0)
        {
            Player.Status.ExtraPerStat -= Data.PerStats[_currentStatIndex];
            Player.Status.ExtraFixedStat -= Data.FixedStats[_currentStatIndex];
            _currentStatIndex = -1;
            return;
        }

        if (CurrentLevel > 1)
        {
            Player.Status.ExtraPerStat -= Data.PerStats[_currentStatIndex];
            Player.Status.ExtraFixedStat -= Data.FixedStats[_currentStatIndex];
        }

        _currentStatIndex++;
        Player.Status.ExtraPerStat += Data.PerStats[_currentStatIndex];
        Player.Status.ExtraFixedStat += Data.FixedStats[_currentStatIndex];
    }
}

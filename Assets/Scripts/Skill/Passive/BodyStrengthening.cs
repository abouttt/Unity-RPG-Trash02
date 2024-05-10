using System.Text;
using UnityEngine;

public class BodyStrengthening : PassiveSkill
{
    public BodyStrengthening(PassiveSkillData data, int level = 0)
        : base(data, level)
    { }

    protected override void RefreshStatDescription()
    {
        var sb = new StringBuilder(50);

        if (CurrentLevel > 0)
        {
            sb.AppendLine($"현재 레벨 : 체력, 공격력, 방어력 {Data.PerStats[CurrentLevel - 1].HP}% 증가");
        }

        if (CurrentLevel < Data.MaxLevel)
        {
            sb.AppendLine($"다음 레벨 : 체력, 공격력, 방어력 {Data.PerStats[CurrentLevel].HP}% 증가");
        }

        Data.StatDescription = sb.ToString();
    }
}

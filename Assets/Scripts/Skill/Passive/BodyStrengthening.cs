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
            sb.AppendLine($"���� ���� : ü��, ���ݷ�, ���� {Data.PerStats[CurrentLevel - 1].HP}% ����");
        }

        if (CurrentLevel < Data.MaxLevel)
        {
            sb.AppendLine($"���� ���� : ü��, ���ݷ�, ���� {Data.PerStats[CurrentLevel].HP}% ����");
        }

        Data.StatDescription = sb.ToString();
    }
}

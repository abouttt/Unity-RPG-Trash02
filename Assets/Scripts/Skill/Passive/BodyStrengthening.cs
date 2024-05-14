using UnityEngine;

public class BodyStrengthening : PassiveSkill
{
    public BodyStrengthening(PassiveSkillData data, int level = 0)
        : base(data, level)
    { }

    protected override void RefreshStatDescription()
    {
        SB.Clear();

        if (CurrentLevel > 0)
        {
            SB.AppendLine($"���� ���� : ü��, ���ݷ�, ���� {Data.PerStats[CurrentLevel - 1].HP}% ����");
        }

        if (CurrentLevel < Data.MaxLevel)
        {
            SB.AppendLine($"���� ���� : ü��, ���ݷ�, ���� {Data.PerStats[CurrentLevel].HP}% ����");
        }

        Data.StatDescription = SB.ToString();
    }
}

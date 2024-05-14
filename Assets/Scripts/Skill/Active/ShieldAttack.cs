using UnityEngine;
using EnumType;

public class ShieldAttack : ActiveSkill
{
    public ShieldAttack(ActiveSkillData data, int level = 0)
        : base(data, level)
    { }

    public override bool Use()
    {
        if (!CheckCanUse())
        {
            return false;
        }

        Player.Animator.SetInteger("SkillNumber", (int)SkillName.ShieldAttack);
        Player.Animator.SetBool("Skill", true);

        return true;
    }

    protected override void RefreshStatDescription()
    {
        SB.Clear();

        if (CurrentLevel > 0)
        {
            SB.AppendLine($"���� ���� : ���� ���ݷ� + {Data.FixedStats[CurrentLevel - 1].Damage} ���ݷ� ����");
        }

        if (CurrentLevel < Data.MaxLevel)
        {
            SB.AppendLine($"���� ���� : ���� ���ݷ� + {Data.FixedStats[CurrentLevel].Damage} ���ݷ� ����");
        }

        Data.StatDescription = SB.ToString();
    }

    protected override bool CheckCanUse()
    {
        if (!Player.EquipmentInventory.IsEquipped(EquipmentType.Shield))
        {
            return false;
        }

        if (!Player.Combat.CanSkill)
        {
            return false;
        }

        return base.CheckCanUse();
    }
}

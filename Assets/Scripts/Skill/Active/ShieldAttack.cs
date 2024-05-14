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
            SB.AppendLine($"현재 레벨 : 현재 공격력 + {Data.FixedStats[CurrentLevel - 1].Damage} 공격력 피해");
        }

        if (CurrentLevel < Data.MaxLevel)
        {
            SB.AppendLine($"다음 레벨 : 현재 공격력 + {Data.FixedStats[CurrentLevel].Damage} 공격력 피해");
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

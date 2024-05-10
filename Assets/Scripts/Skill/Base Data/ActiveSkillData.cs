using UnityEngine;
using EnumType;

public abstract class ActiveSkillData : SkillData, ICooldownable
{
    [field: SerializeField]
    public int RequiredHP { get; private set; }

    [field: SerializeField]
    public int RequiredMP { get; private set; }

    [field: SerializeField]
    public int RequiredSP { get; private set; }

    [field: SerializeField]
    public Cooldown Cooldown { get; set; }

    public ActiveSkillData()
    {
        SkillType = SkillType.Active;
    }
}

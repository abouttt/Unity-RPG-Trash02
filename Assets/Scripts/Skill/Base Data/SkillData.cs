using UnityEngine;
using AYellowpaper.SerializedCollections;
using EnumType;

public abstract class SkillData : ScriptableObject
{
    [field: SerializeField]
    public string SkillID { get; private set; }

    [field: SerializeField]
    public string SkillName { get; private set; }

    [field: SerializeField]
    public Sprite SkillImage { get; private set; }

    [field: SerializeField, ReadOnly]
    public SkillType SkillType { get; protected set; }

    [field: SerializeField, TextArea]
    public string Description { get; private set; }

    [field: SerializeField, TextArea]
    public string StatDescription { get; set; }

    [field: SerializeField]
    public int MaxLevel { get; private set; }

    [field: SerializeField]
    public int LimitLevel { get; private set; }

    [field: SerializeField]
    public int RequiredSkillPoint { get; private set; }

    [field: SerializeField]
    public PlayerStat[] PerStats { get; private set; }

    [field: SerializeField]
    public PlayerStat[] FixedStats { get; private set; }

    [field: SerializeField]
    public bool Root { get; private set; }

    [field: SerializeField, SerializedDictionary("�ڽ� ��ų", "���� ����")]
    public SerializedDictionary<SkillData, int> Children { get; private set; }

    public abstract Skill CreateSkill(int level);

    public bool Equals(SkillData other)
    {
        if (other == null)
        {
            return false;
        }

        if (GetType() != other.GetType())
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return SkillID.Equals(other.SkillID);
    }
}

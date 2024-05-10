using System.Collections.Generic;
using UnityEngine;

public class SkillTree : BaseMonoBehaviour
{
    [SerializeField]
    private SkillData[] _skillDatas;
    private readonly Dictionary<SkillData, Skill> _skills = new();
    private readonly List<Skill> _rootSkills = new();

    private void Awake()
    {
        Player.Status.SkillPointChanged += CheckRootSkills;
        Init();
    }

    private void Start()
    {
        CheckRootSkills();
    }

    public void CheckRootSkills()
    {
        foreach (var skill in _rootSkills)
        {
            skill.CheckState();
        }
    }

    public Skill GetSkillByData(SkillData skillData)
    {
        if (_skills.TryGetValue(skillData, out var skill))
        {
            return skill;
        }

        return null;
    }

    public void ResetAllSkill()
    {
        int totalSkillPoint = 0;
        foreach (var skill in _rootSkills)
        {
            totalSkillPoint += skill.ResetSkill();
        }

        Player.Status.SkillPoint += totalSkillPoint;
    }

    private void Init()
    {
        // ��ų ���� �� ��Ʈ ��ų ����
        foreach (var skillData in _skillDatas)
        {
            var skill = skillData.CreateSkill();
            _skills.Add(skillData, skill);
            if (skillData.Root)
            {
                _rootSkills.Add(skill);
            }
        }

        // �ڽ� ��ų ����
        foreach (var skillData in _skillDatas)
        {
            foreach (var kvp in skillData.Children)
            {
                _skills[skillData].AddChild(_skills[kvp.Key], kvp.Value);
            }
        }
    }
}

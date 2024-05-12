using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Structs;

public class SkillTree : BaseMonoBehaviour, ISavable
{
    public static string SaveKey => "SaveSkillTree";

    [SerializeField]
    private SkillData[] _skillDatas;
    private readonly List<Skill> _rootSkills = new();
    private readonly Dictionary<SkillData, Skill> _skills = new();
    private readonly Dictionary<SkillData, int> _saveSkillLevels = new();

    private void Awake()
    {
        Player.Status.SkillPointChanged += CheckRootSkills;
        Load();
        Init();
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

    public JToken GetSaveData()
    {
        var saveData = new JArray();

        foreach (var kvp in _skills)
        {
            var skillSaveData = new SkillSaveData()
            {
                SkillID = kvp.Key.SkillID,
                CurrentLevel = kvp.Value.CurrentLevel,
            };

            saveData.Add(JObject.FromObject(skillSaveData));
        }

        return saveData;
    }

    private void Init()
    {
        // 스킬 생성 및 루트 스킬 설정
        foreach (var skillData in _skillDatas)
        {
            _saveSkillLevels.TryGetValue(skillData, out int level);
            var skill = skillData.CreateSkill(level);
            _skills.Add(skillData, skill);
            if (skillData.Root)
            {
                _rootSkills.Add(skill);
            }
        }

        // 자식 스킬 설정
        foreach (var skillData in _skillDatas)
        {
            foreach (var kvp in skillData.Children)
            {
                _skills[skillData].AddChild(_skills[kvp.Key], kvp.Value);
            }
        }
    }

    public void Load()
    {
        if (!Managers.Data.Load<JArray>(SaveKey, out var saveData))
        {
            return;
        }

        foreach (var token in saveData)
        {
            var skillSaveData = token.ToObject<SkillSaveData>();
            if (skillSaveData.CurrentLevel == 0)
            {
                continue;
            }

            var skillData = SkillDatabase.GetInstance.FindSkillByID(skillSaveData.SkillID);
            _saveSkillLevels.Add(skillData, skillSaveData.CurrentLevel);
        }
    }
}

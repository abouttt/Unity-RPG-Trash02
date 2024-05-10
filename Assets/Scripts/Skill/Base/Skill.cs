using System;
using System.Collections.Generic;
using UnityEngine;
using EnumType;

public abstract class Skill
{
    public event Action SkillChanged;
    public SkillData Data { get; private set; }
    public bool IsUnlocked { get; private set; }
    public bool IsAcquirable { get; private set; }
    public int CurrentLevel { get; private set; }
    public IReadOnlyList<Skill> Parents => _parents;
    public IReadOnlyDictionary<Skill, int> Children => _children;

    private readonly List<Skill> _parents = new();
    private readonly Dictionary<Skill, int> _children = new();

    public Skill(SkillData data, int level = 0)
    {
        Data = data;
        CurrentLevel = level;
    }

    protected abstract void RefreshStatDescription();

    public void CheckState()
    {
        if (IsUnlocked)
        {
            foreach (var kvp in Children)
            {
                kvp.Key.CheckState();
            }
        }

        if (CurrentLevel == Data.MaxLevel)
        {
            return;
        }

        if (!IsAcquirable)
        {
            if (!CheckParentsLevel())
            {
                return;
            }
        }

        if (Player.Status.Level >= Data.LimitLevel)
        {
            if (!IsAcquirable)
            {
                IsAcquirable = true;
            }

            SkillChanged?.Invoke();
        }
    }

    public void LevelUp()
    {
        if (CurrentLevel == Data.MaxLevel)
        {
            return;
        }

        if (!IsAcquirable)
        {
            return;
        }

        if (!IsUnlocked)
        {
            IsUnlocked = true;
        }

        CurrentLevel++;
        RefreshStatDescription();
        Player.Status.SkillPoint -= Data.RequiredSkillPoint;
        Managers.Quest.ReceiveReport(Category.Skill, Data.SkillID, 1);
        SkillChanged?.Invoke();
    }

    public int ResetSkill()
    {
        int skillPoint = CurrentLevel;

        if (IsUnlocked)
        {
            foreach (var element in _children)
            {
                skillPoint += element.Key.ResetSkill();
            }

            Managers.Quest.ReceiveReport(Category.Skill, Data.SkillID, -CurrentLevel);
            SkillChanged?.Invoke();
        }

        CurrentLevel = 0;
        IsUnlocked = false;
        IsAcquirable = false;

        return skillPoint;
    }

    public void AddChild(Skill skill, int limitLevel)
    {
        if (_children.TryGetValue(skill, out _))
        {
            return;
        }

        _children.Add(skill, limitLevel);
        skill._parents.Add(this);
    }

    private bool CheckParentsLevel()
    {
        foreach (var parents in _parents)
        {
            if (parents.CurrentLevel < parents._children[this])
            {
                return false;
            }
        }

        return true;
    }
}

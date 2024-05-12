using System;
using System.Collections.Generic;
using UnityEngine;
using EnumType;
using Structs;

public class Quest
{
    public event Action TargetCountChanged;
    public QuestData Data { get; private set; }
    public QuestState State { get; private set; } = QuestState.Inactive;
    public IReadOnlyDictionary<QuestTarget, int> Targets => _targets;

    private readonly Dictionary<QuestTarget, int> _targets = new();

    public Quest(QuestData questData)
    {
        Data = questData;
        State = QuestState.Active;

        foreach (var target in questData.Targets)
        {
            int count = 0;

            switch (target.Category)
            {
                case Category.Item:
                    count = Player.ItemInventory.GetSameItemCountByID(target.TargetID);
                    break;
                default:
                    break;
            }

            _targets.Add(target, count);
        }

        CheckCompletable();
    }

    public Quest(QuestSaveData saveData)
    {
        Data = QuestDatabase.GetInstance.FindQuestByID(saveData.QuestID);
        State = saveData.State;

        foreach (var target in Data.Targets)
        {
            foreach (var kvp in saveData.Targets)
            {
                if (target.TargetID.Equals(kvp.Key))
                {
                    _targets.Add(target, kvp.Value);
                    saveData.Targets.Remove(kvp.Key);
                    break;
                }
            }
        }

        CheckCompletable();
    }

    public bool ReceiveReport(Category category, string id, int count)
    {
        if (State == QuestState.Inactive ||
            State == QuestState.Complete)
        {
            return false;
        }

        if (count == 0)
        {
            return false;
        }

        bool isChanged = false;

        foreach (var target in Data.Targets)
        {
            if (target.Category != category)
            {
                continue;
            }

            if (!target.TargetID.Equals(id))
            {
                continue;
            }

            _targets[target] += count;
            isChanged = true;
        }

        if (isChanged)
        {
            CheckCompletable();
            TargetCountChanged?.Invoke();
        }

        return isChanged;
    }

    public bool Complete()
    {
        if (State != QuestState.Completable)
        {
            return false;
        }

        State = QuestState.Complete;
        TargetCountChanged = null;

        Player.Status.Gold += Data.RewardGold;
        Player.Status.XP += Data.RewardXP;

        foreach (var element in Data.RewardItems)
        {
            Player.ItemInventory.AddItem(element.Key, element.Value);
        }

        foreach (var element in _targets)
        {
            var target = element.Key;
            if (target.Category != Category.Item || !target.RemoveAfterCompletion)
            {
                continue;
            }

            Player.ItemInventory.RemoveItem(element.Key.TargetID, element.Key.CompleteCount);
        }

        return true;
    }

    public void Cancel()
    {
        State = QuestState.Inactive;
        TargetCountChanged = null;
    }

    public void CheckCompletable()
    {
        if (State == QuestState.Inactive ||
            State == QuestState.Complete)
        {
            return;
        }

        foreach (var kvp in _targets)
        {
            if (kvp.Key.CompleteCount > kvp.Value)
            {
                State = QuestState.Active;
                return;
            }
        }

        State = QuestState.Completable;
    }
}

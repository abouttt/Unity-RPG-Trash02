using System.Collections.Generic;
using System.Text;
using UnityEngine;
using EnumType;

public class UI_QuestPopup : UI_Popup
{
    enum GameObjects
    {
        QuestInfo,
    }

    enum RectTransforms
    {
        QuestTitleSubitems,
        QuestRewardSlots,
    }

    enum Texts
    {
        QuestTitleText,
        QuestDescriptionText,
        QuestTargetText,
        QuestRewardText,
        NOQuestText,
    }

    enum Buttons
    {
        CloseButton,
        CompleteButton,
        CancelButton,
    }

    private Quest _selectedQuestRef;
    private readonly Dictionary<Quest, UI_QuestTitleSubitem> _titleSubitems = new();
    private readonly StringBuilder _sb = new(50);

    protected override void Init()
    {
        base.Init();

        BindObject(typeof(GameObjects));
        BindRT(typeof(RectTransforms));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.CloseButton).onClick.AddListener(Managers.UI.Close<UI_QuestPopup>);
        GetButton((int)Buttons.CompleteButton).onClick.AddListener(() => Managers.Quest.Complete(_selectedQuestRef));
        GetButton((int)Buttons.CancelButton).onClick.AddListener(() => Managers.Quest.Unregister(_selectedQuestRef));

        Managers.Quest.QuestRegistered += OnQuestRegisterd;
        Managers.Quest.QuestUnregistered += OnQuestCompletedOrCanceled;
        Managers.Quest.QuestCompletabled += OnQuestCompletabled;
        Managers.Quest.QuestCompletableCanceled += OnQuestCompletableCanceld;
        Managers.Quest.QuestCompleted += OnQuestCompletedOrCanceled;

        Clear();
    }

    private void Start()
    {
        Managers.UI.Register<UI_QuestPopup>(this);

        foreach (var quest in Managers.Quest.ActiveQuests)
        {
            OnQuestRegisterd(quest);
            if (quest.State == QuestState.Completable)
            {
                if (_titleSubitems.TryGetValue(quest, out var subitem))
                {
                    subitem.SetActiveCompleteText(true);
                    SetActiveCompleteButton(quest, _selectedQuestRef == quest);
                }
            }
        }
    }

    public void SetQuest(Quest quest)
    {
        if (quest == null)
        {
            return;
        }

        if (_selectedQuestRef == quest)
        {
            return;
        }

        Clear();

        _selectedQuestRef = quest;
        quest.TargetCountChanged += RefreshTargetText;
        GetObject((int)GameObjects.QuestInfo).SetActive(true);
        GetText((int)Texts.QuestTitleText).text = quest.Data.QuestName;
        GetText((int)Texts.QuestDescriptionText).text = quest.Data.Description;
        GetText((int)Texts.NOQuestText).gameObject.SetActive(false);
        GetButton((int)Buttons.CancelButton).gameObject.SetActive(true);
        SetActiveCompleteButton(quest, quest.State == QuestState.Completable);
        RefreshTargetText();
        SetReward(quest.Data);
    }

    public void SetActiveQuestTracker(Quest quest, bool active)
    {
        if (_titleSubitems.TryGetValue(quest, out var subitem))
        {
            subitem.SetActiveQuestTracker(active);
        }
    }

    private void OnQuestRegisterd(Quest quest)
    {
        var go = Managers.Resource.Instantiate("UI_QuestTitleSubitem.prefab", GetRT((int)RectTransforms.QuestTitleSubitems), true);
        var subitem = go.GetComponent<UI_QuestTitleSubitem>();
        subitem.SetQuest(quest);
        _titleSubitems.Add(quest, subitem);
    }

    private void OnQuestCompletabled(Quest quest)
    {
        if (_titleSubitems.TryGetValue(quest, out var subitem))
        {
            subitem.SetActiveCompleteText(true);
            SetActiveCompleteButton(quest, _selectedQuestRef == quest);
        }
    }

    private void OnQuestCompletableCanceld(Quest quest)
    {
        if (_titleSubitems.TryGetValue(quest, out var subitem))
        {
            subitem.SetActiveCompleteText(false);
            SetActiveCompleteButton(quest, !(_selectedQuestRef == quest));
        }
    }

    private void OnQuestCompletedOrCanceled(Quest quest)
    {
        if (_titleSubitems.TryGetValue(quest, out var subitem))
        {
            subitem.SetActiveCompleteText(false);
            subitem.SetActiveQuestTracker(false);
            Managers.Resource.Destroy(subitem.gameObject);
            _titleSubitems.Remove(quest);
            if (_selectedQuestRef == quest)
            {
                Clear();
            }
        }
    }

    private void RefreshTargetText()
    {
        _sb.Clear();
        _sb.AppendLine("[목적]");

        foreach (var kvp in _selectedQuestRef.Targets)
        {
            int completeCount = kvp.Key.CompleteCount;
            int currentCount = Mathf.Clamp(kvp.Value, 0, completeCount);
            _sb.AppendLine($"- {kvp.Key.Description} ({currentCount}/{completeCount})");
        }

        GetText((int)Texts.QuestTargetText).text = _sb.ToString();
    }

    private void SetReward(QuestData questData)
    {
        _sb.Clear();
        _sb.AppendLine("[보상]");

        if (questData.RewardGold > 0)
        {
            _sb.AppendLine($"{questData.RewardGold} Gold");
        }

        if (questData.RewardXP > 0)
        {
            _sb.AppendLine($"{questData.RewardXP} XP");
        }

        GetText((int)Texts.QuestRewardText).text = _sb.ToString();

        foreach (var kvp in questData.RewardItems)
        {
            var go = Managers.Resource.Instantiate("UI_QuestRewardSlot.prefab", GetRT((int)RectTransforms.QuestRewardSlots), true);
            go.GetComponent<UI_QuestRewardSlot>().SetReward(kvp.Key, kvp.Value);
        }
    }

    private void SetActiveCompleteButton(Quest quest, bool active)
    {
        if (active && quest.Data.CanRemoteComplete)
        {
            GetButton((int)Buttons.CompleteButton).gameObject.SetActive(true);
        }
        else
        {
            GetButton((int)Buttons.CompleteButton).gameObject.SetActive(false);
        }
    }

    private void Clear()
    {
        if (_selectedQuestRef != null)
        {
            _selectedQuestRef.TargetCountChanged -= RefreshTargetText;
            _selectedQuestRef = null;
        }

        GetObject((int)GameObjects.QuestInfo).SetActive(false);
        GetText((int)Texts.NOQuestText).gameObject.SetActive(true);
        GetButton((int)Buttons.CompleteButton).gameObject.SetActive(false);
        GetButton((int)Buttons.CancelButton).gameObject.SetActive(false);

        foreach (Transform slot in GetRT((int)RectTransforms.QuestRewardSlots))
        {
            if (slot.gameObject == GetText((int)Texts.QuestRewardText).gameObject)
            {
                continue;
            }

            Managers.Resource.Destroy(slot.gameObject);
        }
    }
}

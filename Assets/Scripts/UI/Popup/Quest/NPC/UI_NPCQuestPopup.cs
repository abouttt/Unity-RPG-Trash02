using System.Collections.Generic;
using System.Text;
using UnityEngine;
using EnumType;

public class UI_NPCQuestPopup : UI_Popup
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
        AcceptButton,
        CompleteButton,
    }

    private NPC _npcRef;
    private QuestData _selectedQuestDataRef;
    private readonly Dictionary<QuestData, UI_NPCQuestTitleSubitem> _titleSubitems = new();
    private readonly StringBuilder _sb = new(50);

    protected override void Init()
    {
        base.Init();

        BindObject(typeof(GameObjects));
        BindRT(typeof(RectTransforms));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.CloseButton).onClick.AddListener(Managers.UI.Close<UI_NPCQuestPopup>);

        GetButton((int)Buttons.AcceptButton).onClick.AddListener(() =>
        {
            var quest = Managers.Quest.Register(_selectedQuestDataRef);
            if (quest.State == QuestState.Completable)
            {
                _titleSubitems[_selectedQuestDataRef].SetActiveCompleteText(true);
                Clear();
            }
            else
            {
                ClearWithSelectedSubitem();
            }
        });

        GetButton((int)Buttons.CompleteButton).onClick.AddListener(() =>
        {
            var quest = Managers.Quest.GetActiveQuest(_selectedQuestDataRef);
            if (quest.State != QuestState.Completable)
            {
                return;
            }

            Managers.Quest.Complete(quest);
            ClearWithSelectedSubitem();
        });

        Player.Status.LevelChanged += () =>
        {
            if (_npcRef != null)
            {
                SetNPC(_npcRef);
            }
        };

        Clear();
    }

    private void Start()
    {
        Managers.UI.Register<UI_NPCQuestPopup>(this);

        Showed += () =>
        {
            Managers.UI.Get<UI_NPCMenuPopup>().PopupRT.gameObject.SetActive(false);
        };

        Closed += () =>
        {
            foreach (var kvp in _titleSubitems)
            {
                Managers.Resource.Destroy(kvp.Value.gameObject);
            }

            _npcRef = null;
            _titleSubitems.Clear();
            Clear();
            Managers.UI.Get<UI_NPCMenuPopup>().PopupRT.gameObject.SetActive(true);
        };
    }

    public void SetNPC(NPC npc)
    {
        _npcRef = npc;

        foreach (var questData in npc.Quests)
        {
            if (Player.Status.Level < questData.LimitLevel)
            {
                continue;
            }

            bool hasPrerequisiteQuests = false;
            foreach (var prerequisiteQuestData in questData.PrerequisiteQuests)
            {
                if (Managers.Quest.GetCompleteQuest(prerequisiteQuestData) == null)
                {
                    hasPrerequisiteQuests = true;
                    break;
                }
            }

            if (hasPrerequisiteQuests)
            {
                continue;
            }

            if (_titleSubitems.TryGetValue(questData, out var _))
            {
                continue;
            }

            var go = Managers.Resource.Instantiate("UI_NPCQuestTitleSubitem.prefab", GetRT((int)RectTransforms.QuestTitleSubitems), true);
            var subitem = go.GetComponent<UI_NPCQuestTitleSubitem>();
            subitem.SetQuestData(questData);
            _titleSubitems.Add(questData, subitem);
        }
    }

    public void SetQuestDescription(QuestData questData)
    {
        if (_selectedQuestDataRef != null && _selectedQuestDataRef.Equals(questData))
        {
            return;
        }

        Clear();

        _selectedQuestDataRef = questData;
        GetObject((int)GameObjects.QuestInfo).SetActive(true);
        GetText((int)Texts.QuestTitleText).text = questData.QuestName;
        GetText((int)Texts.QuestDescriptionText).text = questData.Description;
        GetText((int)Texts.NOQuestText).gameObject.SetActive(false);

        var quest = Managers.Quest.GetActiveQuest(questData);
        if (quest != null && quest.State == QuestState.Completable)
        {
            RefreshTargetText(questData, true);
            GetButton((int)Buttons.CompleteButton).gameObject.SetActive(true);
        }
        else
        {
            RefreshTargetText(questData, false);
            GetButton((int)Buttons.AcceptButton).gameObject.SetActive(true);
        }

        SetReward(questData);
    }

    private void RefreshTargetText(QuestData questData, bool showCurrentCount)
    {
        _sb.Clear();
        _sb.AppendLine("[����]");

        foreach (var target in questData.Targets)
        {
            if (showCurrentCount)
            {
                int completeCount = target.CompleteCount;
                int currentCount = Mathf.Clamp(Managers.Quest.GetActiveQuest(questData).Targets[target], 0, completeCount);
                _sb.AppendLine($"{target.Description} ({currentCount}/{completeCount})");
            }
            else
            {
                _sb.AppendLine($"{target.Description} (0/{target.CompleteCount})");
            }
        }

        GetText((int)Texts.QuestTargetText).text = _sb.ToString();
    }

    private void SetReward(QuestData questData)
    {
        _sb.Clear();
        _sb.AppendLine("[����]");

        if (questData.RewardGold > 0)
        {
            _sb.AppendLine($"{questData.RewardGold} Gold");
        }

        if (questData.RewardXP > 0)
        {
            _sb.AppendLine($"{questData.RewardXP} XP");
        }

        GetText((int)Texts.QuestRewardText).text = _sb.ToString();

        foreach (var element in questData.RewardItems)
        {
            var go = Managers.Resource.Instantiate("UI_QuestRewardSlot.prefab", GetRT((int)RectTransforms.QuestRewardSlots), true);
            go.GetComponent<UI_QuestRewardSlot>().SetReward(element.Key, element.Value);
        }
    }

    private void ClearWithSelectedSubitem()
    {
        Managers.Resource.Destroy(_titleSubitems[_selectedQuestDataRef].gameObject);
        _titleSubitems.Remove(_selectedQuestDataRef);
        Clear();
    }

    private void Clear()
    {
        _selectedQuestDataRef = null;
        GetObject((int)GameObjects.QuestInfo).SetActive(false);
        GetText((int)Texts.NOQuestText).gameObject.SetActive(true);
        GetButton((int)Buttons.AcceptButton).gameObject.SetActive(false);
        GetButton((int)Buttons.CompleteButton).gameObject.SetActive(false);

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

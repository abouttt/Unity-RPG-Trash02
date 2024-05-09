using System.Collections.Generic;
using UnityEngine;
using EnumType;

public class NPC : Interactive
{
    [field: SerializeField]
    public string NPCID { get; private set; }

    [field: SerializeField]
    public string NPCName { get; private set; }

    public IReadOnlyList<QuestData> Quests => _quests;
    public IReadOnlyCollection<BaseNPCMenu> Menus => _menus;

    private static readonly Dictionary<string, NPC> s_NPCs = new();

    [SerializeField, ReadOnly]
    private List<QuestData> _quests;

    [SerializeField]
    private Vector3 _questNotifierPosition = new(0f, 2.3f, 0f);

    [SerializeField]
    private Vector3 _questNotifierInteractionPosition = new(0f, 2.7f, 0f);

    private bool _originCanInteraction;

    private BaseNPCMenu[] _menus;
    private GameObject _questPresenceNotifier;
    private GameObject _questCompletableNotifier;

    protected override void Awake()
    {
        base.Awake();

        s_NPCs.Add(NPCID, this);
        _quests = QuestDatabase.GetInstance.FindQuestsByOwnerID(NPCID);
        _menus = GetComponents<BaseNPCMenu>();
        _originCanInteraction = CanInteraction;
    }

    private void Start()
    {
        InstantiateMinimapIcon("NPCMinimapIcon.sprite", NPCName);

        var pos = transform.position + _questNotifierPosition;
        _questPresenceNotifier = Managers.Resource.Instantiate("QuestPresenceNotifier.prefab", pos, transform);
        _questCompletableNotifier = Managers.Resource.Instantiate("QuestCompletableNotifier.prefab", pos, transform);

        CheckQuests();
    }

    public static bool TryAddQuestToNPC(string id, QuestData questData)
    {
        if (s_NPCs.TryGetValue(id, out var npc))
        {
            npc._quests.Add(questData);
            npc.CheckQuests();
            return true;
        }

        return false;
    }

    public static bool TryRemoveQuestToNPC(string id, QuestData questData)
    {
        if (s_NPCs.TryGetValue(id, out var npc))
        {
            npc._quests.Remove(questData);
            npc.CheckQuests();
            return true;
        }

        return false;
    }

    public override void Interaction()
    {
        Managers.UI.Show<UI_NPCMenuPopup>().SetNPC(this);
    }

    private void CheckQuests()
    {
        _questPresenceNotifier.SetActive(false);
        _questCompletableNotifier.SetActive(false);

        int lockedQuestCount = 0;
        bool hasCompletableQuest = false;

        foreach (var questData in _quests)
        {
            if (questData.LimitLevel > Player.Status.Level)
            {
                lockedQuestCount++;
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
                lockedQuestCount++;
                continue;
            }

            var quest = Managers.Quest.GetActiveQuest(questData);
            if (quest == null)
            {
                continue;
            }

            if (quest.State == QuestState.Completable)
            {
                hasCompletableQuest = true;
                break;
            }
        }

        if (Quests.Count != lockedQuestCount)
        {
            _questPresenceNotifier.SetActive(!hasCompletableQuest);
            _questCompletableNotifier.SetActive(hasCompletableQuest);
        }

        if (!CanInteraction && (_questPresenceNotifier.activeSelf || _questCompletableNotifier.activeSelf))
        {
            CanInteraction = true;
        }
        else if (CanInteraction && !_originCanInteraction)
        {
            CanInteraction = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsDetected)
        {
            return;
        }

        if (Player.InteractionDetector.IsShowedKeyGuide)
        {
            _questPresenceNotifier.transform.localPosition = _questNotifierInteractionPosition;
            _questCompletableNotifier.transform.localPosition = _questNotifierInteractionPosition;
        }
        else
        {
            _questPresenceNotifier.transform.localPosition = _questNotifierPosition;
            _questCompletableNotifier.transform.localPosition = _questNotifierPosition;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _questPresenceNotifier.transform.localPosition = _questNotifierPosition;
        _questCompletableNotifier.transform.localPosition = _questNotifierPosition;
    }

    protected override void OnDestroy()
    {
        s_NPCs.Remove(NPCID);
        base.OnDestroy();
    }
}

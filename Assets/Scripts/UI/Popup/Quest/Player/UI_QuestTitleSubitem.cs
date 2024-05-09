using UnityEngine;
using UnityEngine.UI;

public class UI_QuestTitleSubitem : UI_Base
{
    enum Texts
    {
        TitleText,
        CompleteText,
    }

    enum Buttons
    {
        TitleButton,
    }

    enum Toggles
    {
        QuestTrackerToggle,
    }

    public Quest QuestRef { get; private set; }

    protected override void Init()
    {
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        Bind<Toggle>(typeof(Toggles));

        GetButton((int)Buttons.TitleButton).onClick.AddListener(() => Managers.UI.Get<UI_QuestPopup>().SetQuest(QuestRef));
        GetText((int)Texts.CompleteText).gameObject.SetActive(false);
        Get<Toggle>((int)Toggles.QuestTrackerToggle).isOn = false;
    }

    public void SetQuest(Quest quest)
    {
        QuestRef = quest;
        GetText((int)Texts.TitleText).text = $"[{quest.Data.LimitLevel}] {quest.Data.QuestName}";
    }

    public void SetActiveCompleteText(bool active)
    {
        GetText((int)Texts.CompleteText).gameObject.SetActive(active);
    }

    public void SetActiveQuestTracker(bool active)
    {
        if (Get<Toggle>((int)Toggles.QuestTrackerToggle).isOn != active)
        {
            Get<Toggle>((int)Toggles.QuestTrackerToggle).isOn = active;
        }
    }

    public bool IsShowedTracker()
    {
        return Get<Toggle>((int)Toggles.QuestTrackerToggle).isOn;
    }
}
